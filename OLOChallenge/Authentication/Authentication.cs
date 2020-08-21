using OLOChallenge.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLOChallenge.Authentication
{
    /// <summary>
    /// If I were working with a real API that accepted posts I would expect it to be behind an authentication
    /// gate, as such I'm creating a class to handle user authentication and return a session access object.
    /// </summary>
    public static class Auth
    {
        /// <remarks>
        /// A real authentication method might take a userid and password, possibly an environmental endpoint, it might also talk to the server...
        /// </remarks>
        public static SessionObject Authenticate(int userid)
        {
            var so = new SessionObject() { UserID = userid.ToString() };
            Log.WriteDebug($"Authenticated with credentials {so.UserID}");
            return so;
        }
    }
}
