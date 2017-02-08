using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;
using LoginManagement;
using LoginManagement.Exceptions;
using System.Web.Security;
using System.Text;
using Newtonsoft.Json;


namespace WebUI.Controllers
{   
    [Authorize]
    public class AdminController : Controller
    {
        private ILoginManagement _service;
        private SectionProfileModel model;
        private UserListModel userListModel;
        private ProfileSoftwareModel profileSofwareModel;


        public AdminController()
        {
            this._service = new LoginManagementImpl();
            List<Section> sections = _service.GetAllSection();
            List<Profile> profiles = _service.GetAllProfile();
            model = new SectionProfileModel { Profiles = profiles, Sections = sections };
            userListModel = new UserListModel { Users = _service.GetAllUser() };
            List<Software> softwares = _service.GetAllSoftware();
            profileSofwareModel = new ProfileSoftwareModel { Profiles = profiles, Softwares = softwares };
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
            return View(model);
        }

        public ActionResult ListUser()
        {
            return View();
        }

        // creer publc-ic action resut
        public ActionResult ProfileManagement()
        {
            return View(profileSofwareModel);
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
            
            return View("UserManagement", model);
        }

        [HttpPost]
        public ViewResult AddUser(string type, string lastName, string firstname,
            string email, int refNumber,int year, int section, int profile)
        {
            _service.AddUser(type, lastName, firstname, email, refNumber, year, section, profile);
            return View("UserManagement", model);
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

        [HttpPost]
        public ActionResult AddProfileType(string typeProfile, List<string> softwares)
        {
            try {
                _service.AddProfileType(typeProfile, softwares);
                TempData["SuccessMessage"] = "Création réussie";
            } catch (ArgumentException exp)
            {
                TempData["ErrorMessage"] = "Echec de la création";
            }
            return RedirectToAction("ProfileManagement", profileSofwareModel);
        }

        [HttpPost]
        public ViewResult ModifyProfileType(string typeProfile, List<String> softwares)
        {
            try
            {
                this._service.modifyProfileType(typeProfile, softwares);
                TempData["SuccessMessage"] = "Modification réussie";
            }catch (Exception exp)
            {
                TempData["ErrorMessage"] = "Echec de la modification";
            }
            
            return View("ProfileManagement", profileSofwareModel);
        }

        [HttpPost]
        public ActionResult RemoveProfileType(string typeProfile)
        {
            this._service.removeProfileType(typeProfile);
            return RedirectToAction("ProfileManagement", profileSofwareModel);
        }

    }
}