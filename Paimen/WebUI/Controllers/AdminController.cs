using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginManagement;
using LoginManagement.Exceptions;

namespace WebUI.Controllers
{
    public class AdminController : Controller
    {
        private ILoginManagement _service;

        public AdminController()
        {
            this._service = new LoginManagementImpl();
        }

        // GET: UserManager
        public ActionResult UserManagement()
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
                Console.WriteLine("ECHEC!");
            }
            
            return View("UserManagement");
        }
    }
}