using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace coffee_shop.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Admin()
        {
            ViewBag.Title = "Admin Page";

            return View();
        }
    }
}