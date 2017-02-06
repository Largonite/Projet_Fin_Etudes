using LoginManagement.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace LoginManagement
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class LoginManagementImpl : ILoginManagement
    {
        private GenericDao<User> _userDao;

        public LoginManagementImpl()
        {
            PaimenEntities entities = new PaimenEntities();
            this._userDao = new GenericDao<User>(entities);
        }

        public User SignIn(User user)
        {
            User inDbUser = this._userDao.Find(u => u.RegNumber.Equals(user.RegNumber));
            if(inDbUser == null)
            {
                return null;
            }
            if (!string.IsNullOrEmpty(user.Password))
            {
                if (!user.Password.Equals(inDbUser.Password))
                {
                    return null;
                }
            }
            return inDbUser;
        }
    }
}
