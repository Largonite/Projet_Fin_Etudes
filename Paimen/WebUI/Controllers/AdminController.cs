﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginManagement;
using LoginManagement.Exceptions;
using System.Web.Security;
using System.Text;
using WebUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ILoginManagement _service;
        public static List<Section> _sections;
        public static List<Profile> _profiles;
        public static List<Software> _softwares;
        public static List<User> _users;

        public AdminController()
        {
            this._service = new LoginManagementImpl();
            _sections = _service.GetAllSection();
            _profiles = _service.GetAllProfile();
            _softwares = _service.GetAllSoftware();
            _users = _service.GetAllUser();
        }

        public ActionResult Index()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            
            return View((object) ticket.Name);
        }

        // GET: UserManager
        public ActionResult UserManagement()
        {
            return View();
        }

        public ViewResult SoftwareManagement()
        {
            return View(_softwares);
        }

        public ActionResult DeleteSoftware(int id, string name)
        {
            bool res = _service.DeleteSofwtare(id);
            if (res)
            {
                TempData["SuccessMessage"] = string.Format("{0} avec l'id {1} à été supprimé", name, id);
            }
            else
            {
                TempData["ErrorMessage"] = string.Format("Impossible de supprimer {0} !", name);
            }
            return RedirectToAction("SoftwareManagement");

        }

        public string SaveSoftware(Software s)
        {
            Software s1 = this._service.GetAllSoftware().Where(soft => soft.Id == s.Id).First();
            string oldName = s1.Name;
            s.Profiles = s1.Profiles;
            bool r = this._service.SaveSoftware(s);
            if (r)
            {
                TempData["SuccessMessage"] = string.Format("{0} a été modifié en {1}", oldName,s.Name);
            }
            else
            {
                TempData["ErrorMessage"] = string.Format("Impossible de modifier {0} !", s1.Name);
            }
            return "SoftwareManagement";
        }

        public ActionResult AddSoftware(string name)
        {
            Software s = new Software { Name = name };
            if (this._service.AddSoftware(s))
            {
                TempData["SuccessMessage"] = string.Format("{0} a été ajouté ",name);
            }
            else
            {
                TempData["ErrorMessage"] = string.Format("Impossible d'ajouter {0} !", name);
            }
            return RedirectToAction("SoftwareManagement");
        }

        [HttpPost]
        public ViewResult AddUserFromCSV(HttpPostedFileBase csv)
        {
            try
            {
                _service.AddStudentFromCSV(csv);
            }
            catch (DBException exception)
            {
                Console.WriteLine(exception.Message);
            }
            
            return View("UserManagement");
        }

        [HttpPost]
        public ViewResult AddUser(string type, string lastName, string firstname,
            string email, Nullable<int> regNumber, Nullable<int> year, Nullable<int> section, int profile)
        {
            bool ajout = false; 
            try
            {
                ajout = _service.AddUser(type, lastName, firstname, email, regNumber, year, section, profile);
            }
            catch(Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }
            if (ajout)
                TempData["SuccessMessage"] = "Ajout effectué";
            return View("UserManagement");
        }

        [HttpGet]
        public void DownloadBat()
        {
            IDictionary<Section, List<int>> sections = new Dictionary<Section, List<int>>();
            int year;
            string sectionString;
            foreach (string param in Request.QueryString.AllKeys)
            {
                int.TryParse(param.Substring(0, 1), out year);
                sectionString = param.Substring(1);
                Section sec = _sections.FirstOrDefault(s => s.Code.Equals(sectionString));
                if (sec != null)
                {
                    if (sections.ContainsKey(sec))
                    {
                        sections[sec].Add(year);
                    }
                    else
                    {
                        List<int> years = new List<int>();
                        years.Add(year);
                        sections.Add(sec, years);
                    }
                }
            }
            byte[] script = Encoding.ASCII.GetBytes(this._service.GetWindowsScript(null, sections));

            this.Response.ContentType = "application/octet-stream";
            this.Response.AddHeader("Content-Disposition", "attachment; filename=addUsers.bat");
            this.Response.OutputStream.Write(script, 0, script.Length);
            this.Response.Flush();
        }

        [HttpGet]
        public void DownloadClaroline()
        {
            byte[] script = Encoding.ASCII.GetBytes(this._service.GetClarolineScript(null, null));

            this.Response.ContentType = "application/octet-stream";
            this.Response.AddHeader("Content-Disposition", "attachment; filename=clarolineUsers.csv");
            this.Response.OutputStream.Write(script, 0, script.Length);
            this.Response.Flush();
        }

        [HttpGet]
        public void DownloadNutriLog()
        {
            byte[] script = Encoding.ASCII.GetBytes(this._service.GetNutrilogScript(null, null));

            this.Response.ContentType = "application/octet-stream";
            this.Response.AddHeader("Content-Disposition", "attachment; filename=nutrilogUsers.csv");
            this.Response.OutputStream.Write(script, 0, script.Length);
            this.Response.Flush();
        }

        [HttpGet]
        public string GetSections()
        {
            return JsonConvert.SerializeObject(this._service.GetAllSection());
        }
    }
}