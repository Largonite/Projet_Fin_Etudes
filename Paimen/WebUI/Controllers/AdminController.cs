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
        private SectionProfileModel sectionProfileModel;
        private UserListModel userListModel;
        //private IDictionary<int, Action<IDictionary<Section, List<int>>>> downloads;

        public AdminController()
        {
            this._service = new LoginManagementImpl();
            List<Section> sections = _service.GetAllSections();
            List<Profile> profiles = _service.GetAllProfiles();
            SectionProfileModel.Profiles = profiles;
            SectionProfileModel.Sections = sections;
            SectionProfileModel.Softwares = _service.GetAllSoftwares();
            //model = new SectionProfileModel();// { Profiles = profiles, Sections = sections };
            userListModel = new UserListModel { Users = _service.GetAllUser() };
            //this.downloads = new Dictionary<int, Action<IDictionary<Section, List<int>>>>();
            //downloads.Add(1, this.DownloadBat);
            //downloads.Add(2, this.DownloadClaroline);
            //downloads.Add(3, this.DownloadNutriLog);
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
            IList<Software> softwares = _service.GetAllSoftwares();
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
            Software s1 = this._service.GetAllSoftwares().Where(soft => soft.Id == s.Id).First();
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
        public void Download()
        {
            List<int> softwaresPks = new List<int>();
            IDictionary<Section, List<int>> sections = this.GetConstraints();
            string softwarePK = Request.QueryString["radioSoftware"];
            Software software = this._service.GetAllSoftwares().First(s => s.Id == int.Parse(softwarePK));
            switch (software.Name)
            {
                case "Windows": this.DownloadBat(sections);
                    break;
                case "Nutrilog": this.DownloadNutriLog(sections);
                    break;
                case "Claroline": this.DownloadClaroline(sections);
                    break;
                default: return;
            }
            //int temp;
            //Request.QueryString.AllKeys.Where(key => int.TryParse(key, out temp)).ToList().ForEach(pk => softwaresPks.Add(int.Parse(pk)));
            //softwaresPks.ForEach(pk => this.downloads[pk].Invoke(sections));
        }

        private IDictionary<Section, List<int>> GetConstraints()
        {
            IDictionary<Section, List<int>> sections = new Dictionary<Section, List<int>>();
            int year;
            string sectionString;
            foreach (string param in Request.QueryString.AllKeys)
            {
                if (param.Length > 1 && int.TryParse(param.Substring(0, 1), out year))
                {
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
            }
            return sections;
        }

        [HttpGet]
        public void DownloadBat(IDictionary<Section, List<int>> sections)
        {
            
            byte[] script = Encoding.ASCII.GetBytes(this._service.GetWindowsScript(null, sections));

            this.Response.ContentType = "application/octet-stream";
            this.Response.AddHeader("Content-Disposition", "attachment; filename=addUsers.bat");
            this.Response.OutputStream.Write(script, 0, script.Length);
            this.Response.Flush();
        }

        [HttpGet]
        public void DownloadClaroline(IDictionary<Section, List<int>> sections)
        {
            byte[] script = Encoding.ASCII.GetBytes(this._service.GetClarolineScript(null, sections));

            this.Response.ContentType = "application/octet-stream";
            this.Response.AddHeader("Content-Disposition", "attachment; filename=clarolineUsers.csv");
            this.Response.OutputStream.Write(script, 0, script.Length);
            this.Response.Flush();
        }

        [HttpGet]
        public void DownloadNutriLog(IDictionary<Section, List<int>> sections)
        {
            byte[] script = Encoding.ASCII.GetBytes(this._service.GetNutrilogScript(null, sections));

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