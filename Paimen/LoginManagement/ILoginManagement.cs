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

        void CreateProfileType(string typeProfil);


    }
}
