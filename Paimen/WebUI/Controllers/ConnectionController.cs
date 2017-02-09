using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginManagement;
using WebUI.Models;
using System.Web.Security;
using System.IO;
using System.Diagnostics;

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


        [HttpGet]
        public void DownloadPDF(int idUser)
        {
            /*
             *             this.Response.ContentType = "application/octet-stream";
            this.Response.AddHeader("Content-Disposition", "attachment; filename=addUsers.bat");
            this.Response.OutputStream.Write(script, 0, script.Length);
            this.Response.Flush();*/

            //String filename = _service.GetPDFForStudent(idUser);
            //Debug.WriteLine(filename);
            byte[] pdf = this._service.GetPDFForStudent(idUser);
            User u = this._service.GetAllUser().FirstOrDefault(us => us.Id == idUser);
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=login_" + u.FirstName + "_" + u.LastName + "_" + ".pdf");
            Response.OutputStream.Write(pdf, 0, pdf.Length);
            Response.End();
        }
    }
}