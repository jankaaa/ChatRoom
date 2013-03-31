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
        public static List<Users> UsersOnline = new List<Users>();

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
                if (!(userName.Length > 4))
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
                Clients.AllExcept(Context.ConnectionId).sendToGlobal("(" + DateTime.Now.ToShortTimeString() +
                    ") " + userName.Name, msg);
                Clients.Caller.sendToGlobal("(" + DateTime.Now.ToShortTimeString() +
                    ") <span class=\"SenderFormat\">" + userName.Name + "</span>", msg);
            }
            else
            {
                SendErrorMessage("Lai aktivizētu tērzēšanas funkcijas jums ir jāauterizējas sistēmā!");
            }
        }

        /// <summary>
        /// Klientam izejot no sistēmas
        /// </summary>
        public override System.Threading.Tasks.Task OnDisconnected()
        {
            //Noskaidrojam lietotāju kurš izgājis no sistēmas(izsaucis šo metodi)
            RemovePersonFromGroup();
            return base.OnDisconnected();
        }

        /// <summary>
        /// Izņem lietotāju no saraksta
        /// </summary>
        public void RemovePersonFromGroup()
        {
            var user = UsersOnline.FirstOrDefault(x => x.Connection == Context.ConnectionId);
            UsersOnline.Remove(user);
            Clients.All.onDisconnection(Context.ConnectionId, user.Name);
        }

        /// <summary>
        /// Sūtīt privātu ziņu
        /// </summary>
        /// <param name="message">Ziņas teksts</param>
        /// <param name="toConnId">Kam sūtīt</param>
        public void SendPrivateMessage(string message, string toConnId)
        {
            var Sender = UsersOnline.FirstOrDefault(x => x.Connection == Context.ConnectionId);
            //Nevajadzētu būt, bet ja nu tomēr
            if (Sender != null)
            {

                Clients.Client(toConnId).SendPrivateMessage(Sender.Connection, "(" + DateTime.Now.ToShortTimeString() +
                    ") " + Sender.Name, message);
                //Ja pats sev sūta.
                if (toConnId != Context.ConnectionId)
                {
                    Clients.Caller.SendPrivateMessage(toConnId, "(" + DateTime.Now.ToShortTimeString() +
          ") <span class=\"SenderFormat\">" + Sender.Name + "</span>", message);
                }
            }
            else
            {
                Clients.Caller.SendPrivateMessage(toConnId, "(" + DateTime.Now.ToShortTimeString() +
") <span style=\"Color:Red\" class=\"SenderFormat\">System</span>", "Lai sūtītu ziņu ir jāienāk sistēmā!");
            }

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
            if (user == null)
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

        /// <summary>
        /// Kļūdu paziņojums
        /// </summary>
        /// <param name="error">Kļūdu ziņa</param>
        public void SendErrorMessage(string error)
        {
            Clients.Caller.SendErrorMessage("Error", error);
        }

        #endregion

    }
}