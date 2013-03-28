using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ChatRoom.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string id)
        {
            if (id.Length > 4 && id.Length < 11)
            {
                FormsAuthentication.SetAuthCookie(id, false);
            }
            return Redirect("~/Account/Index");
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/Account/Index");
        }


    }
}
