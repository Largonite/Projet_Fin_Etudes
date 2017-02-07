using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginManagement;
using System.Web.Security;
using System.Text;

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
    }
}