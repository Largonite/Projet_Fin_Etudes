using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginManagement;
using WebUI.Models;
namespace WebUI.Controllers
{
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
            model = new SectionProfileModel { Profiles = profiles, Sections = sections };
            userListModel = new UserListModel { Users = _service.GetAllUser() };
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
            return View(model);
        }

        public ActionResult ListUser()
        {
            return View();
        }

        [HttpPost]
        public ViewResult AddUserFromCSV(HttpPostedFileBase csv)
        {
            return View("UserManagement", model);
        }

        [HttpPost]
        public ViewResult AddUser(string type, string lastName, string firstname,
            string email, string login, string password, string refNumber,
            int year, int section, int profile)
        {

            return View("UserManagement", model);
        }



    }
}