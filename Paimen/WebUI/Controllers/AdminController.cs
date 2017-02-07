using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;
using LoginManagement;

namespace WebUI.Controllers
{
    public class AdminController : Controller
    {
        private ILoginManagement _service;
        private ProfileSoftwareModel profileSofwareModel;

        public AdminController()
        {
            this._service = new LoginManagementImpl();
            List<Profile> profiles = _service.GetAllProfile();
            List<Software> softwares = _service.GetAllSoftware();
            profileSofwareModel = new ProfileSoftwareModel { Profiles = profiles, Softwares = softwares };
        }

        public ActionResult Index()
        {
            User u = (User)TempData["Admin"];
            if (u == null) return RedirectToAction("SignIn", "Connection");
            return View(u);
        }

        // GET: UserManager
        public ActionResult UserManagement()
        {
            return View();
        }

        // creer publc-ic action resut
        public ActionResult ProfilManagement()
        {
            return View(profileSofwareModel);
        }

        [HttpPost]
        public ViewResult AddUserFromCSV(HttpPostedFileBase csv)
        {
            return View("UserManagement");
        }

        [HttpPost]
        public ViewResult AddProfileType(string typeProfile, List<string> software)
        {
            return View("ProfilManagement");
        }

    }
}