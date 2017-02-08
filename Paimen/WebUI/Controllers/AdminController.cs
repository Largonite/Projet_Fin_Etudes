using System;
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


namespace WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ILoginManagement _service;
        private SectionProfileModel sectionProfileModel;
        private UserListModel userListModel;


        public AdminController()
        {
            this._service = new LoginManagementImpl();
            List<Section> sections = _service.GetAllSection();
            List<Profile> profiles = _service.GetAllProfile();
            sectionProfileModel = new SectionProfileModel { Profiles = profiles, Sections = sections };
            userListModel = new UserListModel { Users = _service.GetAllUser() };
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
            return View(sectionProfileModel);
        }

        public ViewResult SoftwareManagement()
        {
            IList<Software> softwares = _service.GetAllSoftware();
            return View(softwares);
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

        public ActionResult ListUser()
        {
            return View();
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
            
            return View("UserManagement", sectionProfileModel);
        }

        [HttpPost]
        public ViewResult AddUser(string type, string lastName, string firstname,
            string email, int refNumber,int year, int section, int profile)
        {
            _service.AddUser(type, lastName, firstname, email, refNumber, year, section, profile);
            return View("UserManagement", sectionProfileModel);
        }

        [HttpGet]
        public void DownloadBat()
        {
            byte[] script = Encoding.ASCII.GetBytes(this._service.GetWindowsScript(null, null));

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
            return JsonConvert.SerializeObject(this._service.GetSections());
        }
    }
}