using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class ConnectionController : Controller
    {

        public ViewResult SignIn()
        {
            return View();
        }
        // GET: Connexion
        public ActionResult Index()
        {
            return View();
        }
    }
}