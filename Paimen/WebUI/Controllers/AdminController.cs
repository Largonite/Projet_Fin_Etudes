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
using System.Diagnostics;

namespace WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ILoginManagement _service;
        private SectionProfileModel model;
        private UserListModel userListModel;


        public AdminController()
        {
            this._service = new LoginManagementImpl();
            List<Section> sections = _service.GetAllSection();
            List<Profile> profiles = _service.GetAllProfile();
            SectionProfileModel.Profiles = profiles;
            SectionProfileModel.Sections = sections;
            SectionProfileModel.Softwares = _service.GetAllSoftwares();
            model = new SectionProfileModel();// { Profiles = profiles, Sections = sections };
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
            return View(model);
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
            IDictionary<Section, List<int>> sections = new Dictionary<Section, List<int>>();
            int year;
            string sectionString;
            foreach (string param in Request.QueryString.AllKeys)
            {
                int.TryParse(param.Substring(0, 1), out year);
                sectionString = param.Substring(1);
                Section sec = SectionProfileModel.Sections.FirstOrDefault(s => s.Code.Equals(sectionString));
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
            return JsonConvert.SerializeObject(this._service.GetSections());
        }
    }
}