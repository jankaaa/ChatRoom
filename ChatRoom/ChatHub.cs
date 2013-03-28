using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChatRoom.HubCommon;
using System.Web.Security;

namespace ChatRoom
{
    public class ChatHub : Hub
    {
        # region "Properties"

        //Aktīvie lietotāji sistēmā
        static List<Users> UsersOnline = new List<Users>();

        #endregion

        #region "Hub Methods"

        /// <summary>
        /// Klientam ienākot sistēmā
        /// </summary>
        /// <param name="userName"></param> 
        public override System.Threading.Tasks.Task OnConnected()
        {
            AddPersonToGroup(null);
            return base.OnConnected();
        }

        /// <summary>
        /// pievieno lietotāju sarakstam
        /// </summary>
        /// <param name="userName"></param>
        public void AddPersonToGroup(string userName)
        {
            bool validUser = true;
            if (userName == null)
            {
                userName = Context.Request.User.Identity.Name;
                //Nav autorizējies.
                if (!(userName.Length > 5))
                    validUser = false;

            }
            else if (userName.Length < 5 || userName.Length > 10)
            {
                Clients.Caller.SendErrorAlert("Lietotāja vārda garumam jābūt robezās no 5 līdz 10 simboliem !");
                validUser = false;
            }
            //Ja lietotāja vārds ir atbilstošs, tad piereģistrē
            if (validUser)
            {
                AddNewUser(userName);
            }
            //Atgriežam lietotājus, kas ir tiešsasitē
            Clients.All.onConnection(UsersOnline);
            
           
        }

        /// <summary>
        /// Sūtīt ziņu lielājā istabā
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="msg"></param>
        public void GlobalMessages(string msg)
        {
            var userName = UsersOnline.FirstOrDefault(x => x.Connection == Context.ConnectionId);
            if (userName != null)
            {
                Clients.All.sendToGlobal(userName.Name, msg);
            }
            else
            {
                SendErrorMessage("Lai aktivizētu tērzēšanas funkcijas jums ir jāauterizējas !");
            }
        }

        /// <summary>
        /// Kļūdu paziņojums
        /// </summary>
        /// <param name="error">Kļūdu ziņa</param>
        public void SendErrorMessage(string error)
        {
            Clients.Caller.SendErrorMessage("Error", error);
        }

        /// <summary>
        /// Sūtīt ziņu privāti
        /// </summary>
        /// <param name="toUserId"></param>
        /// <param name="msg"></param>
        public void PrivateMessage(string toUserId, string msg)
        {
            var fromUserId = Context.ConnectionId;
            var toUser = UsersOnline.FirstOrDefault(x => x.Connection == toUserId);
            var fromUser = UsersOnline.FirstOrDefault(x => x.Connection == fromUserId);
            if (toUser != null && fromUser != null)
            {
                //Nosūta klientam
                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.Name, msg);
                //Nosūta pats sev atpakaļ, lai pārliecinatos ka ir nosūtīts klietam
                Clients.Caller.sendPrivateMessage(toUserId, fromUser.Name, msg);
            }

        }

        /// <summary>
        /// PAr cik logins ir uz Callback, piereģistrējam jaunu lietotāju caur šo metodi
        /// </summary>
        /// <returns></returns>
        public void OnReconnected()
        {
            var userName = Context.Request.User.Identity.Name;
            if (userName.Length > 4)
            {
                //Saņem jaunpienākošā lietotāja Id. guid formātā.
                var userID = Context.ConnectionId;
                Users user = new Users() { Name = userName, Connection = userID };
                var userExsists = UsersOnline.FirstOrDefault(x => x.Connection == userID);
                if (userExsists == null)
                {
                    UsersOnline.Add(user);
                    //Nosūtām jaunpienākošā lietotāja informāciju
                    Clients.Caller.onConnection(userID, userName, UsersOnline);
                    //Nosūtam pārējiem informāciju, ka pienācis jauns lietotājs.
                    Clients.AllExcept(userID).onNewUserConnection(userID, userName);
                }
            }
        }

        /// <summary>
        /// Klientam izejot no sistēmas
        /// </summary>
        public override System.Threading.Tasks.Task OnDisconnected()
        {
            //Noskaidrojam lietotāju kurš izgājis no sistēmas(izsaucis šo metodi)
            RemovePersonToGroup();
            return base.OnDisconnected();
        }

        /// <summary>
        /// Izņem lietotāju no saraksta
        /// </summary>
        public void RemovePersonToGroup()
        {
            var user = UsersOnline.FirstOrDefault(x => x.Connection == Context.ConnectionId);
            UsersOnline.Remove(user);
            Clients.All.onDisconnection(Context.ConnectionId, user.Name);
        }

        #endregion

        #region "Functions"

        /// <summary>
        /// Pirms piešķirt lietotāja vārdu, apskatāmies vai tāds jau nav izveidots. Ja ir - pieliekam cipariņus
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private string ValidateUserName(string userName, int Ident)
        {
            var user = UsersOnline.FirstOrDefault(x => x.Name == userName);
            if(user == null)
            {
                return userName;
            }
            else
            {
                userName = userName.Remove(userName.Length - Ident.ToString().Length) + Ident;
                return ValidateUserName(userName, Ident + 1);
            }
        }

        /// <summary>
        /// Pievieno jaunu klientu
        /// </summary>
        /// <param name="userName"></param>
        public void AddNewUser(string userName)
        {
            //Saņem jaunpienākošā lietotāja Id. guid formātā.
            var userID = Context.ConnectionId;
            Users user = new Users() { Name = ValidateUserName(userName, 0), Connection = userID };
            UsersOnline.Add(user);
        }

        #endregion

    }
}