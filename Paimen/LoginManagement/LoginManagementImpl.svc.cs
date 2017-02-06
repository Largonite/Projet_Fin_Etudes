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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class LoginManagementImpl : ILoginManagement
    {
        public void add()
        {
            throw new NotImplementedException();
        }

        public bool AddStudentFromCSV(string csv)
        {
            string[] lines = File.ReadAllLines(csv);

            Section[] sections = GetAllSections();

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

                User newStudent = new User(Convert.ToInt32(user[0]), user[1], user[2], Convert.ToInt32(user[3]), idSection, user[5], "Student", login, password);
            }
            return true;
        }

        private Section[] GetAllSections()
        {
            return null;
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
    }
}
