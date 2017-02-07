using LoginManagement.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using DevOne.Security.Cryptography.BCrypt;

namespace LoginManagement
{
    public class LoginManagementImpl : ILoginManagement
    {
        private GenericDao<User> _userDao;
        private GenericDao<Section> _sectionDao;

        public LoginManagementImpl()
        {
            PaimenEntities entities = new PaimenEntities();
            this._userDao = new GenericDao<User>(entities);
            this._sectionDao = new GenericDao<Section>(entities);
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

        public bool AddStudentFromCSV(string csv)
        {
            string[] lines = File.ReadAllLines(csv);

            IList<Section> sections = GetAllSections();

            int idSection = 0;
            foreach(string line in lines)
            {
                string[] user = line.Split(',');

                foreach(Section s in sections)
                {
                    if (s.Code == user[4])
                    {
                        idSection = s.Id;
                    }
                }

                string login = GetLogin(user[2], user[1]);

                string password = GenerateAndEncryptPassword();

                User newStudent = new User
                {
                    RegNumber = Convert.ToInt32(user[0]),
                    LastName = user[1],
                    FirstName = user[2],
                    Year = Convert.ToInt32(user[3]),
                    Section = idSection,
                    Email = user[5],
                    Type = "Student",
                    Login = login,
                    Password = password
                };

                this._userDao.Add(newStudent);
            }
            return true;
        }

        private IList<Section> GetAllSections()
        {
            return this._sectionDao.GetAll();
        }

        private string GetLogin(string firstName, string lastName)
        {
            char beginning = (firstName.ToArray())[0];
            string end = lastName.Substring(0, Math.Min(lastName.Length, 6)); // Math.Min => si jamais le lastName est inférieur à 6 lettres 

            return beginning + end;
        }

        private string GenerateAndEncryptPassword()
        {
            return BCryptHelper.HashPassword(System.Web.Security.Membership.GeneratePassword(10, 5), BCryptHelper.GenerateSalt());
        }

        public string GetWindowsScript(DateTime d, IDictionary<Section, List<int>> sections)
        {
            List<User> users = new List<User>();
            foreach(KeyValuePair<Section, List<int>> entry in sections)
            {
                users.AddRange(this._userDao.FindAll(user => user.Type.Equals("Student") 
                                                             && d == null ? true : d.Date <= user.AddedDate.Date
                                                             && user.Section.Equals(entry.Key) 
                                                             && entry.Value.Contains(user.Year.Value)));
            }

            StringBuilder builder = new StringBuilder();
            foreach(User u in users)
            {
                builder.Append("dsadd ");
                builder.Append(u.LastName);
                builder.Append(" /prenom=");
                builder.Append(u.FirstName);
                builder.Append(" /mdp=");
                builder.Append(u.Password);
                builder.Append("\n");
            }
            return builder.ToString();
        }
    }
}
