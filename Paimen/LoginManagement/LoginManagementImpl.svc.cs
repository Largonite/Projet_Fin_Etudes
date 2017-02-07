using LoginManagement.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Web;
using LoginManagement.Exceptions;
using System.Text.RegularExpressions;

namespace LoginManagement
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class LoginManagementImpl : ILoginManagement
    {
        private GenericDao<User> _userDao;
        private GenericDao<Section> _sectionDao;
        private GenericDao<Profile> _profilDao;

        public LoginManagementImpl()
        {
            PaimenEntities entities = new PaimenEntities();
            this._userDao = new GenericDao<User>(entities);
            this._sectionDao = new GenericDao<Section>(entities);
            this._profilDao = new GenericDao<Profile>(entities);
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


        // Add students from CSV
        public bool AddStudentFromCSV(HttpPostedFileBase csv)
        {
            string fileContent = new StreamReader(csv.InputStream).ReadToEnd();
            string pattern = "\"";
            string replacement = "";
            Regex regex = new Regex(pattern);
            fileContent = regex.Replace(fileContent, replacement);
            string[] lines = fileContent.Split('\n');
            Console.WriteLine("" + lines.ToString());
            IList<Section> sections = this._sectionDao.GetAll();

            int idSection = 0;
          
            for(int i = 1; i < lines.Length - 1; i++)
            {
                string[] user = lines[i].Split(',');

                foreach(Section s in sections)
                {
                    if (String.Equals(s.Code, user[4]))
                    {
                        idSection = s.Id;
                    }
                }

                string login = GetLogin(user[2], user[1]);
                string profileName = (user[3].ToArray())[0] + user[4];
                Profile profile = this._profilDao.Find(p => p.Name.Equals(profileName));
                string password = System.Web.Security.Membership.GeneratePassword(10, 5);

                string annee = user[3].Substring(0,1);
                int anneeInt = Convert.ToInt32(annee);

        User newStudent = new User
                {
                    RegNumber = Convert.ToInt32(user[0]),
                    LastName = user[1],
                    FirstName = user[2],
                    Year = Convert.ToInt32(user[3].Substring(0,1)),
                    Section = idSection,
                    Email = user[5],
                    Type = "Student",
                    Login = login,
                    Password = password,
                    Profile = profile.Id,
                };
                if (!this._userDao.Add(newStudent))
                {
                    throw new DBException("Une erreur est survenue durant l'ajout d'un etudiant!");
                }
                this._userDao.SaveChanges();
            }     
            return true;
        }

        private string GetLogin(string firstName, string lastName)
        {
            char beginning = (firstName.ToArray())[0];
            string end = lastName.Substring(0, Math.Min(lastName.Length, 6)); // Math.Min => si jamais le lastName est inférieur à 6 lettres 
            string login = beginning + end;

            if(this._userDao.Find(u => u.Login == login) != null)
            {
               login = (firstName.Substring(0, 2)) + end;
            }

            return login;
        }

        /*
        public bool AddProfileForGuest(int guestId, string profileName, IList<int> IdSoftwares)
        {
            Profile guestProfile = new Profile {Name = profileName };

            if (!this._profilDao.Add(guestProfile))
            {
                throw new DBException("Une erreur est survenue durant la création du profil!");
            }
            return false;
        }*/
    }
}
