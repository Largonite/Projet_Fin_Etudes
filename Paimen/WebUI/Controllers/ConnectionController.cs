using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginManagement;
using WebUI.Models;
using System.Web.Security;

namespace WebUI.Controllers
{
    public class ConnectionController : Controller
    {
        private ILoginManagement _service;
        public ConnectionController()
        {
            this._service = new LoginManagementImpl();
        }

        public ViewResult LogIn()
        {
            return View();
        }

        public void LogOut()
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        // GET: Connexion
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserFromReg(string regNumber, string password="")
        {

            int regInt;
            if(!int.TryParse(regNumber, out regInt))
            {
                TempData["ErrorMessage"] = "Ce n'est pas un numéro matricule correct!";
                return RedirectToAction("LogIn");
            }

            User res = this._service.SignIn(new User {RegNumber = regInt, Password = password });
            if (res == null)
            {
                TempData["ErrorMessage"] = "Mauvais matricule!!!!!";
                return RedirectToAction("LogIn");
            }
            if (res.Profile1.Name.Equals("Admin"))
            {
                FormsAuthentication.SetAuthCookie(res.FirstName + " " + res.LastName, true);
                return RedirectToAction("UserManagement", "Admin");
            }
            return View("UserInformation",res);
        }

        public ViewResult UserInformation(User u)
        {
            return View(u);
        }
    }
}