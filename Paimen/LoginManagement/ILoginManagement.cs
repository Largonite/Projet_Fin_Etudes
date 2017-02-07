using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;

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
        string GetWindowsScript(DateTime d, IDictionary<Section,List<int>> sections);

        [OperationContract]
        void AddProfileType(string typeProfile, string software);

        [OperationContract]
        bool AddStudentFromCSV(string csv);

    }
}
