using LoginManagement.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LoginManagement.Exceptions;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;

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

        public void AddProfileType(string typeProfile, List<string> softwares)
        {

            if ( this._profileDao.Find(p => p.Name.Equals(typeProfile)) != null)
            {
                throw new ArgumentException("Profile name already used");
            }

            Profile toAddProfile = new Profile();

            toAddProfile.Name = typeProfile;
            this._profileDao.Add(toAddProfile);
            this._profileDao.SaveChanges();

            toAddProfile = this._profileDao.Find(p => p.Name.Equals(typeProfile));

            AddSoftwaresToProfile(toAddProfile, softwares);
            
        }

        private void AddSoftwaresToProfile(Profile profile, List<string> softwares)
        {

            foreach (string software in softwares)
            {
                int idSoft = Convert.ToInt32(software);
                Software softwareInDb = this._softwareDao.Find(s => s.Id.Equals(idSoft));

                profile.Softwares.Add(softwareInDb);

                softwareInDb.Profiles.Add(profile);

            }

            this._profileDao.SaveChanges();

        }

        public void ModifyProfileType(string typeProfile, List<string> softwares)
        {

            int typeProfileId = Convert.ToInt32(typeProfile);
            Profile profile = this._profileDao.Find(p => p.Id.Equals(typeProfileId));
            if (profile == null) throw new ArgumentException("Profile name does not exist");

            // remove the revoked softwares
            foreach (Software s in profile.Softwares.ToArray())
            {
                if( !softwares.Contains("" + s.Id)) profile.Softwares.Remove(s);
            }

            AddSoftwaresToProfile(profile, softwares);
            this._softwareDao.SaveChanges();

        }

        public void RemoveProfileType(string typeProfile)
        {
            int typeProfileId = Convert.ToInt32(typeProfile);
            Profile profile = this._profileDao.Find(p => p.Id.Equals(typeProfileId));
            if (profile == null) return;

            foreach(Software software in profile.Softwares.ToArray())
            {
                profile.Softwares.Remove(software);
            }

            this._profileDao.Delete(profile);
            this._profileDao.SaveChanges();
        }

        public User SignIn(User user)
        {
            User inDbUser = this._userDao.Find(u => u.RegNumber.Value.Equals(user.RegNumber.Value));
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
        public bool AddStudentFromCSV(string fileContent)
        {
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
                try
                {
                    this._userDao.SaveChanges();
                }
                catch(Exception e)
                {
                    Debug.WriteLine("Exception catch pendant le SaveChanges()");
                    Debug.WriteLine(e.Message);
                }
            }     
            return true;
        }

        private string GetLogin(string firstName, string lastName)
        {
            //char beginning = (firstName.ToArray())[0];
            //string pattern = " ";
            //string replacement = "";
            //Regex regex = new Regex(pattern);
            int nbLettersFName = 1;
            firstName = firstName.ToLower();
            lastName = lastName.ToLower();
            lastName.Replace(" ", "");
            firstName.Replace(" ", "");
            //lastName = regex.Replace(lastName, replacement);
            //string end = lastName.Substring(0, Math.Min(lastName.Length, 6)); // Math.Min => si jamais le lastName est inférieur à 6 lettres 
            //string login = beginning + end;
            string login = firstName.Substring(0, Math.Min(nbLettersFName, firstName.Length - 1)) + lastName.Substring(0, Math.Min(lastName.Length, 7 - nbLettersFName));
            
            while(this._userDao.Find(u => u.Login.Equals(login)) != null)
            {
                nbLettersFName++;
                Debug.WriteLine(login);
                login = firstName.Substring(0, nbLettersFName) + lastName.Substring(0, 7 - nbLettersFName);
            }

            return login;
        }

        public string GetWindowsScript(IDictionary<Section, List<int>> sections)
        {
            List<User> users = this.GetUsers(sections);
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

        public string GetNutrilogScript(IDictionary<Section, List<int>> sections)
        {
            List<User> users = this.GetUsers(sections);
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

        public string GetClarolineScript(IDictionary<Section, List<int>> sections)
        {
            List<User> users = this.GetUsers(sections);
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

        public List<Software> GetAllSoftwares()
        {
            return this._softwareDao.GetAll();
        }

        public List<User> GetAllUser()
        {
            return _userDao.GetAll().ToList();
        }

        public bool AddUser(string type, string lastName, string firstname, string email, Nullable<int> refNumber, Nullable<int> year, Nullable<int> section, int profile)
        {
            if (lastName.Equals("")) throw new ArgumentNullException("Le nom ne peut pas être vide.");
            if (firstname.Equals("")) throw new ArgumentNullException("Le prénom ne peut pas être vide.");
            if (profile == 0) throw new ArgumentNullException("Aucun profil n'a été attribué");
            if (refNumber != null && _userDao.Find(u => u.RegNumber.Value.Equals(refNumber.Value)) != null) throw new ArgumentException("Ce matricule existe déjà.");
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
                Profile = profile,
                AddedDate = DateTime.UtcNow.Date
            };

            this._userDao.Add(toAdd);
            this._userDao.SaveChanges();
            return true;
        }

        private List<User> GetUsers(IDictionary<Section, List<int>> sections)
        {
            // Both params are null, return the whole list.
            if (sections == null)
            {
                return this._userDao.GetAll();
            }


            List<User> users = new List<User>();
            foreach (KeyValuePair<Section, List<int>> entry in sections)
            {
                users.AddRange(this._userDao.FindAll(user => user.Section1.Code.Equals(entry.Key.Code)
                                                             && entry.Value.Contains(user.Year.Value)
                ));
            }
            return users;
         }

        public byte[] GetPDFForAllUsers()
        {
            IList<User> listUsers = this._userDao.GetAll();

            if(listUsers.Count == 0)
            {
                throw new NoSuchUserException("Aucun utilisateurs n'a été trouvé!");
            }

            MemoryStream stream = new MemoryStream();
            
            Document sendBack = new Document(PageSize.A4, 25, 25, 30, 30); //Page size and page margin
            PdfWriter writer = PdfWriter.GetInstance(sendBack, stream);

            Image vinci = Image.GetInstance(Properties.Resources.Vinci, System.Drawing.Imaging.ImageFormat.Png);
            Image ipl = Image.GetInstance(Properties.Resources.IPL, System.Drawing.Imaging.ImageFormat.Jpeg);

            sendBack.Open();

            foreach (User user in listUsers)
            {
                Profile profile = user.Profile1;
                Section section = user.Section1;

                vinci.SetAbsolutePosition(1, 700);
                ipl.SetAbsolutePosition(450, 690);
                sendBack.Add(vinci);
                sendBack.Add(ipl);

                sendBack.Add(new Paragraph("\n"));
                sendBack.Add(new Paragraph("\n"));
                sendBack.Add(new Paragraph("\n"));

                Font titleFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 30, Font.BOLD | Font.UNDERLINE);
                Paragraph title = new Paragraph("Feuille de login", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                sendBack.Add(title);

                sendBack.Add(new Paragraph("\n"));
                sendBack.Add(new Paragraph("\n"));
                sendBack.Add(new Paragraph("\n"));

                sendBack.Add(new Paragraph("Prénom : " + user.FirstName));
                sendBack.Add(new Paragraph("Nom : " + user.LastName));
                if (user.Email == null || user.Email.Equals(""))
                {
                    sendBack.Add(new Paragraph("Email : /"));
                }
                else
                {
                    sendBack.Add(new Paragraph("Email : " + user.Email));
                }

                if (user.RegNumber == null || user.RegNumber.Equals(""))
                {
                    sendBack.Add(new Paragraph("Matricule : /"));
                }
                else
                {
                    sendBack.Add(new Paragraph("Matricule : " + user.RegNumber));
                }


                if (section != null)
                {
                    sendBack.Add(new Paragraph("Section : " + section.Name ?? "/"));
                }
                else
                {
                    sendBack.Add(new Paragraph("Section : /"));
                }

                sendBack.Add(new Paragraph("Année : " + user.Year ?? "/"));
                sendBack.Add(new Paragraph("Login : " + user.Login ?? "/"));
                sendBack.Add(new Paragraph("Mot de passe : " + user.Password));

                if (profile != null)
                {
                    sendBack.Add(new Paragraph("Profil : " + profile.Name));
                }
                else
                {
                    sendBack.Add(new Paragraph("Profil : /"));
                }

                sendBack.NewPage();
            }

            sendBack.Close();
            writer.Close();
            
            return stream.ToArray();
        }

        public List<Section> GetAllSection()
        {
            return this._sectionDao.GetAll();
        }

        public bool DeleteSofwtare(int id)
        {
            Software soft = this._softwareDao.Find(s => s.Id == id);
            bool res = this._softwareDao.Delete(soft);
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
  

        public byte[] GetPDFForStudent(int idStudent)
        {
            User user = this._userDao.Find(u => u.Id == idStudent);
            Section section = this._sectionDao.Find(s => s.Id == user.Section);

            if(user == null || user.Profile1 == null)
            {
                throw new NoSuchUserException("Aucun utilisateur ou profil n'a été trouvé!");
            }

            Image vinci = Image.GetInstance(Properties.Resources.Vinci, System.Drawing.Imaging.ImageFormat.Png);
            Image ipl = Image.GetInstance(Properties.Resources.IPL, System.Drawing.Imaging.ImageFormat.Jpeg);

            MemoryStream stream = new MemoryStream();
            Document sendBack = new Document(PageSize.A4, 25, 25, 30, 30); //Page size and page margin
            PdfWriter writer = PdfWriter.GetInstance(sendBack, stream);

            sendBack.Open();

            vinci.SetAbsolutePosition(1, 700);
            ipl.SetAbsolutePosition(450, 690);
            sendBack.Add(vinci);
            sendBack.Add(ipl);

            for(int i = 0; i < 7; i++)
            {
                sendBack.Add(new Paragraph("\n"));
            }

            vinci.SetAbsolutePosition(1, 700);
            ipl.SetAbsolutePosition(450, 690);
            sendBack.Add(vinci);
            sendBack.Add(ipl);

            sendBack.Add(new Paragraph("\n"));
            sendBack.Add(new Paragraph("\n"));
            sendBack.Add(new Paragraph("\n"));

            Font titleFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 30 ,Font.BOLD | Font.UNDERLINE);
            Paragraph title = new Paragraph("Feuille de login", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            sendBack.Add(title);

            sendBack.Add(new Paragraph("\n"));
            sendBack.Add(new Paragraph("\n"));
            sendBack.Add(new Paragraph("\n"));

            sendBack.Add(new Paragraph("Prénom : " + user.FirstName));
            sendBack.Add(new Paragraph("Nom : " + user.LastName));
            if (user.Email == null || user.Email.Equals(""))
            {
                sendBack.Add(new Paragraph("Email : /"));
            }
            else
            {
                sendBack.Add(new Paragraph("Email : " + user.Email));
            }

            if (user.RegNumber == null || user.RegNumber.Equals(""))
            {
                sendBack.Add(new Paragraph("Matricule : /"));
            }
            else
            {
                sendBack.Add(new Paragraph("Matricule : " + user.RegNumber));
            }


            if (section != null)
            {
                sendBack.Add(new Paragraph("Section : " + section.Name ?? "/"));
            }
            else
            {
                sendBack.Add(new Paragraph("Section : /"));
            }

            sendBack.Add(new Paragraph("Année : " + user.Year ?? "/"));
            sendBack.Add(new Paragraph("Login : " + user.Login ?? "/"));
            sendBack.Add(new Paragraph("Mot de passe : " + user.Password));

            if (user.Profile1 != null)
            {
                sendBack.Add(new Paragraph("Profil : " + user.Profile1.Name));
            }
            else
            {
                sendBack.Add(new Paragraph("Profil : /"));
            }


            sendBack.Close();
            writer.Close();

            return stream.ToArray() ;
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

        public bool DeleteUser(int id)
        {
            User us = this._userDao.Find(u=>u.Id == id);
            bool res = this._userDao.Delete(us);
            return res;
        }

        public void SetProfile(IDictionary<Section, List<int>> sections, Profile profile)
        {
            List<User> users = this.GetUsers(sections);
            users.ForEach(u => u.Profile1 = profile);
            users.ForEach(u => this._userDao.Update(u));
        }
    }
}
