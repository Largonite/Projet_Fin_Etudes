using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginManagement; 

namespace WebUI.Controllers
{
    public class ConnectionController : Controller
    {
        private ILoginManagement _service;
        public ConnectionController()
        {
            this._service = new LoginManagementImpl();
        }

        public ViewResult SignIn()
        {
            return View();
        }
        // GET: Connexion
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ViewResult UserFromReg(string reg)
        {
            User res = this._service.SignIn(new User { RegNumber = int.Parse(reg), Password="" });
            
            return View("UserInformation",User);
        }

        public ViewResult UserInformation(User u)
        {
            return View(u);
        }
    }
}