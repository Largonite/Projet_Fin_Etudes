using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginManagement;
using System.Web.Security;

namespace WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ILoginManagement _service;

        public AdminController()
        {
            this._service = new LoginManagementImpl();
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

        [HttpPost]
        public ViewResult AddUserFromCSV(HttpPostedFileBase csv)
        {
            return View("UserManagement");
        }
    }
}