using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatRoom.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetPrivateMessageDialog(string Id)
        {
            if (Id != null)
            {
               
                var userName = ChatHub.UsersOnline.FirstOrDefault(x => x.Connection == Id);
                ViewBag.ConnectionId = Id;
                ViewBag.UserName = userName.Name;
                return View();
            }
            else return null;

        }

    }
}
