using System.Linq;
using System.Text.RegularExpressions;

namespace Crafted.Volva {
    /// <summary>
    /// Helper for basic authentication
    /// </summary>
    internal class BasicAuthenticationHelper {

        /// <summary>
        /// Authenticates the specified username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        internal static bool Authenticate(string username, string password) {
            using(Crafted.Configuration.Config<VolvaConfig> v = new Configuration.Config<VolvaConfig>()) {
                if(v == null) {
                    return true;
                }
                bool isValid = v.Values.Users.Any(n => n.Username == username && n.Password == password);
                if(!isValid && v.Values.allowWindowsUsers) {
                    isValid = (new WindowsLogonApiHelper()).Authenticate(username, password);
                }
                return isValid;
            }
        }

        /// <summary>
        /// Requireses the authentication.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        internal static bool RequiresAuthentication(string url) {
            using(Crafted.Configuration.Config<VolvaConfig> v = new Configuration.Config<VolvaConfig>()) {
                if(v == null) {
                    return true;
                }
                return v.Values.Locations.Any(n => n.matchPattern == "*" || Regex.IsMatch(url, n.matchPattern));
            }
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        internal static string Message {
            get {
                using(Crafted.Configuration.Config<VolvaConfig> v = new Configuration.Config<VolvaConfig>()) {
                    if(v == null) {
                        return string.Empty;
                    }
                    if(v.Values.Message == null) {
                        return "Area secured by Crafted.Volva";
                    }
                    return v.Values.Message.Content;
                }
            }
        }

        /// <summary>
        /// Gets the name of the cookie.
        /// </summary>
        /// <value>
        /// The name of the cookie.
        /// </value>
        internal static string CookieName {
            get {
                using(Crafted.Configuration.Config<VolvaConfig> v = new Configuration.Config<VolvaConfig>()) {
                    if(v == null) {
                        return string.Empty;
                    }
                    return v.Values.CookieName;
                }
            }
        }

        /// <summary>
        /// Gets the cookie expires.
        /// </summary>
        internal static int CookieExpires {
            get {
                using(Crafted.Configuration.Config<VolvaConfig> v = new Configuration.Config<VolvaConfig>()) {
                    if(v == null) {
                        return 0;
                    }
                    return v.Values.CookieExpires;
                }
            }
        }


        /// <summary>
        /// Gets a value indicating whether [enable basic authentication].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable basic authentication]; otherwise, <c>false</c>.
        /// </value>
        internal static bool EnableBasicAuthentication {
            get {
                using(Crafted.Configuration.Config<VolvaConfig> v = new Configuration.Config<VolvaConfig>()) {
                    if(v == null) {
                        return false;
                    }
                    return v.Values.BasicAuthenticationIsEnabled;
                }
            }
        }

    }
}
