using LoginManagement.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using LoginManagement.Exceptions;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace LoginManagement
{
    public class LoginManagementImpl : ILoginManagement
    {
        private GenericDao<User> _userDao;
        private GenericDao<Section> _sectionDao;
        private GenericDao<Profile> _profileDao;
        private GenericDao<Software> _softwareDao;

        public LoginManagementImpl()
        {
            PaimenEntities entities = new PaimenEntities();
            this._userDao = new GenericDao<User>(entities);
            this._sectionDao = new GenericDao<Section>(entities);
            this._profileDao = new GenericDao<Profile>(entities);
            this._softwareDao = new GenericDao<Software>(entities);
        }

        public User SignIn(User user)
        {
            User inDbUser = this._userDao.Find(u => u.RegNumber.Equals(user.RegNumber));
            if (inDbUser == null)
            {
                return null;
            }
            if (inDbUser.Type.Equals("Admin"))
            {
                if(!string.IsNullOrEmpty(user.Password) && user.Password.Equals(inDbUser.Password))
                {
                    return inDbUser;
                }
                return null;
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

                foreach (Section s in sections)
                {
                    if (String.Equals(s.Code, user[4]))
                    {
                        idSection = s.Id;
                    }
                }

                string login = GetLogin(user[2], user[1]);
                string profileName = (user[3].ToArray())[0] + user[4];
                Profile profile = this._profileDao.Find(p => p.Name.Equals(profileName));
                string password = System.Web.Security.Membership.GeneratePassword(10, 5);

                string annee = user[3].Substring(0,1);
                int anneeInt = Convert.ToInt32(annee);

                User newStudent = new User
                {
                    RegNumber = Convert.ToInt32(user[0]),
                    LastName = user[1],
                    FirstName = user[2],
                    Year = Convert.ToInt32(user[3].Substring(0, 1)),
                    Section = idSection,
                    Email = user[5],
                    Type = "Student",
                    Login = login,
                    Password = password,
                    Profile = profile.Id,
                    AddedDate = DateTime.UtcNow.Date,
                };
                if (this._userDao.Add(newStudent) == -1)
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

        public string GetWindowsScript(DateTime? d, IDictionary<Section, List<int>> sections)
        {
            List<User> users = this.GetUsers(d,sections);
            StringBuilder builder = new StringBuilder();
            foreach (User u in users)
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

        public string GetNutrilogScript(DateTime? d, IDictionary<Section, List<int>> sections)
        {
            List<User> users = this.GetUsers(d, sections);
            StringBuilder builder = new StringBuilder();
            builder.Append("Nom;Prenom;Email;Mot de passe;\n");
            foreach (User u in users)
            {
                builder.Append(u.Id);
                builder.Append(";");
                builder.Append(u.LastName);
                builder.Append(";");
                builder.Append(u.FirstName);
                builder.Append(";");
                builder.Append(u.Password);
                builder.Append(";\n");
            }
            return builder.ToString();
            
        }

        public string GetClarolineScript(DateTime? d, IDictionary<Section, List<int>> sections)
        {
            List<User> users = this.GetUsers(d, sections);
            StringBuilder builder = new StringBuilder();
            builder.Append("Nom;Prenom;Email;Mot de passe;\n");
            foreach (User u in users)
            {
                builder.Append(u.LastName);
                builder.Append(";");
                builder.Append(u.FirstName);
                builder.Append(";");
                builder.Append(string.IsNullOrEmpty(u.Email) ? "" : u.Email);
                builder.Append(";");
                builder.Append(u.Password);
                builder.Append(";\n");
            }
            return builder.ToString();
        }

        public List<Section> GetAllSections()
        {
            return _sectionDao.GetAll().ToList();
        }

        public List<Profile> GetAllProfiles()
        {
            return _profileDao.GetAll().ToList();
        }

        public List<User> GetAllUser()
        {
            return _userDao.GetAll().ToList();
        }

        public bool AddUser(string type, string lastName, string firstname, string email,  int refNumber, int year, int section, int profile)
        {
            if (lastName == null) throw new ArgumentNullException("Le nom ne peut pas être vide.");
            if (firstname == null) throw new ArgumentNullException("Le prénom ne peut pas être vide.");
            if (profile == 0) throw new ArgumentNullException("Aucun profil n'a été attribué");
            if (_userDao.Find(u => u.RegNumber.Equals(refNumber)) == null) throw new ArgumentException("Ce matricule existe déjà.");
            User toAdd = new User
            {
                Type = type,
                LastName = lastName,
                FirstName = firstname,
                Email = email,
                Login = GetLogin(firstname, lastName),
                Password = System.Web.Security.Membership.GeneratePassword(10, 5),
                RegNumber = refNumber,
                Year = year,
                Section = section,
                Profile = profile
            };
            _userDao.Add(toAdd);
            _userDao.SaveChanges();
                return true;
        }

        private List<User> GetUsers(DateTime? d, IDictionary<Section, List<int>> sections)
        {
            // Both params are null, return the whole list.
            if (d == null && sections == null)
            {
                return this._userDao.GetAll();
            }

            if(sections == null)
            {
                return this._userDao.FindAll(user => d.Value.Date <= user.AddedDate.Date);
            }

            List<User> users = new List<User>();
            foreach (KeyValuePair<Section, List<int>> entry in sections)
            {
                users.AddRange(this._userDao.FindAll(user => (d == null ? true : DateTime.Compare(user.AddedDate, d.Value) <= 0)
                                                             && user.Section1.Code.Equals(entry.Key.Code)
                                                              && entry.Value.Contains(user.Year.Value)
                ));
            }
            return users;
         }

        public Document GetPDFForAllUsers()
        {
            IList<User> listUsers = this._userDao.GetAll();

            if(listUsers.Count == 0)
            {
                throw new NoSuchUserException("Aucun utilisateurs n'a été trouvé!");
            }

            FileStream fs = new FileStream("Liste utilisateurs - " + DateTime.Now, FileMode.Create);
            Document sendBack = new Document(PageSize.A4, 25, 25, 30, 30); //Page size and page margin
            PdfWriter writer = PdfWriter.GetInstance(sendBack, fs);

            sendBack.Open();

            foreach (User user in listUsers)
            {
                Profile profile = this._profileDao.Find(p => p.GetId() == user.GetId());
                Section section = this._sectionDao.Find(s => s.GetId() == user.Section);

                sendBack.Add(new Paragraph("Prénom : " + user.FirstName));
                sendBack.Add(new Paragraph("Nom : " + user.LastName));
                sendBack.Add(new Paragraph("Email : " + user.Email ?? "/"));
                sendBack.Add(new Paragraph("Matricule : " + user.RegNumber ?? "/"));
                sendBack.Add(new Paragraph("Section : " + section.Name ?? "/"));
                sendBack.Add(new Paragraph("Année : " + user.Year ?? "/"));
                sendBack.Add(new Paragraph("Login : " + user.Login ?? "/"));
                sendBack.Add(new Paragraph("Mot de passe : " + user.Password));
                sendBack.Add(new Paragraph("Profil : " + profile.Name));

                sendBack.NewPage();
            }

            return sendBack;
        }

        public List<Section> GetAllSection()
        {
            return this._sectionDao.GetAll();
        }

        public IList<Software> GetAllSoftwares()
        {
            return this._softwareDao.GetAll();
        }

        public bool DeleteSofwtare(int id)
        {
            Software soft = this._softwareDao.Find(s => s.Id == id);
            bool res = this._softwareDao.Delete(soft);
            if (res)
            {
                this._softwareDao.SaveChanges();
            }
            return res;
        }

        public bool SaveSoftware(Software s)
        {
            if (this._softwareDao.Update(s))
            {
                this._softwareDao.SaveChanges();
                return true;
            }
            return false;
        }
  

        public Document GetPDFForStudent(int idStudent)
        {
            User user = this._userDao.Find(u => u.GetId() == idStudent);
            Profile profile = this._profileDao.Find(p => p.GetId() == user.GetId());
            Section section = this._sectionDao.Find(s => s.GetId() == user.Section);

            if(user == null || profile == null)
            {
                throw new NoSuchUserException("Aucun utilisateur ou profil n'a été trouvé!");
            }

            FileStream fs = new FileStream(user.FirstName + " - " + user.LastName + " - " + DateTime.Now, FileMode.Create);
            Document sendBack = new Document(PageSize.A4, 25, 25, 30, 30); //Page size and page margin
            PdfWriter writer = PdfWriter.GetInstance(sendBack, fs);

            sendBack.Open();

            sendBack.Add(new Paragraph("Prénom : " + user.FirstName));
            sendBack.Add(new Paragraph("Nom : " + user.LastName));
            sendBack.Add(new Paragraph("Email : " + user.Email ?? "/"));
            sendBack.Add(new Paragraph("Matricule : " + user.RegNumber ?? "/"));
            sendBack.Add(new Paragraph("Section : " + section.Name ?? "/"));
            sendBack.Add(new Paragraph("Année : " + user.Year ?? "/"));
            sendBack.Add(new Paragraph("Login : " + user.Login ?? "/"));
            sendBack.Add(new Paragraph("Mot de passe : " + user.Password));
            sendBack.Add(new Paragraph("Profil : " + profile.Name));

            sendBack.Close();
            writer.Close();
            fs.Close();

            return sendBack;
        }

        public bool AddSoftware(Software s)
        {
            if (this._softwareDao.Add(s) != -1)
            {
                this._softwareDao.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
