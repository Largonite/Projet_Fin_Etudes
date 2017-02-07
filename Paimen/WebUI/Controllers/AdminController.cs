using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginManagement;

namespace WebUI.Controllers
{
    public class AdminController : Controller
    {
        private ILoginManagement _service;

        public AdminController()
        {
            this._service = new LoginManagementImpl();
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

        [HttpPost]
        public ViewResult AddUserFromCSV(HttpPostedFileBase csv)
        {
            return View("UserManagement");
        }
    }
}