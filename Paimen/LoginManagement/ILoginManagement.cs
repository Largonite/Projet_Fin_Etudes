using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace LoginManagement
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ILoginManagement
    {
        /// <summary>
        /// Allows a user to sign in. 
        /// </summary>
        /// <param name="user">A user with at least RegNumber initialized. If the Password is also initialized, 
        /// the user is considered to be an Admin.</param>
        /// <returns>The user with full available data in DataBase or 
        /// null either if the user does not exist or if the password does not match</returns>
        [OperationContract]
        User SignIn(User user);

        /// <summary>
        /// Allows to retrieve a .bat Windows script. 
        /// This script contains the command lines to add all the users registered since the specified date (included). 
        /// If the date is not set (==null), the date of today is considered.
        /// A Dictionnary<Section, List<Integer>> can be used to specify the sections to consider and the years for each section.
        /// </summary>
        /// <returns>A string formatted as .bat script to add the specified users</returns>
        [OperationContract]
        string GetWindowsScript(DateTime? d, IDictionary<Section,List<int>> sections);


        /// <summary>
        /// Allows to retrieve a .csv string formatted for Nutrilog. 
        /// Each line is formatted like "idStudent, LastName, FisrtName, Password"
        /// with all the users registered since the specified date (included). 
        /// If the date is not set (== null), the date will not be considered as a criteria.
        /// A Dictionnary<Section, List<Integer>> can be used to specify the sections to consider
        /// and the years for each section. If the Dictionnary is null, it will not be considered as a criteria.
        /// </summary>
        /// <returns>A string formatted as CSV with the specified users</returns>
        [OperationContract]
        string GetNutrilogScript(DateTime? d, IDictionary<Section, List<int>> sections);


        /// <summary>
        /// Allows to retrieve a .csv string formatted for Claroline. 
        /// Each line is formatted like "LastName, FisrtName, Email, Password"
        /// with all the users registered since the specified date (included). 
        /// If the date is not set (== null), the date will not be considered as a criteria.
        /// A Dictionnary<Section, List<Integer>> can be used to specify the sections to consider
        /// and the years for each section. If the Dictionnary is null, it will not be considered as a criteria.
        /// </summary>
        /// <returns>A string formatted as CSV with the specified users</returns>
        [OperationContract]
        string GetClarolineScript(DateTime? d, IDictionary<Section, List<int>> sections);

        [OperationContract]
        bool AddStudentFromCSV(HttpPostedFileBase csv);

        [OperationContract]
        List<Section> GetAllSection();

        [OperationContract]
        List<Profile> GetAllProfile();

        /// <summary>
        /// Allows to retrieve the list of all the softwares in the database
        /// </summary>
        /// <returns>A ILis of Software</returns>
        [OperationContract]
        IList<Software> GetAllSoftware();

        /// <summary>
        /// Allows to delete a software according to its id
        /// </summary>
        /// <returns>true if software is deleted, false otherwise</returns>
        [OperationContract]
        bool DeleteSofwtare(int id);

        [OperationContract]
        bool SaveSoftware(Software s);

        [OperationContract]
        bool AddSoftware(Software s);

        [OperationContract]
        List<User> GetAllUser();

        [OperationContract]
        bool AddUser(string type, string lastName, string firstname,
            string email,  int refNumber, int year, int section, int profile);
        /*[OperationContract]
        bool AddProfileForGuest(int guestId, string profileName, IList<int> IdSoftwares);*/

        [OperationContract]
        List<Section> GetSections();
    }
}
