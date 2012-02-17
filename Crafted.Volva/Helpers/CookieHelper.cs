using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace Crafted.Volva {
    /// <summary>
    /// Encryption for the authentication
    /// </summary>
    internal sealed class AuthenticationEncryption {
        private const string KEY = "sfdjf48mdfdf3054";

        internal static string Encrypt(string plainText) {
            string encrypted = null;
            byte[] inputBytes = Encoding.ASCII.GetBytes(plainText);
            byte[] pwdhash = null;

            //generate an MD5 hash from the password. 
            //a hash is a one way encryption meaning once you generate
            //the hash, you cant derive the password back from it.
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            pwdhash = hashmd5.ComputeHash(Encoding.ASCII.GetBytes(KEY));
            hashmd5 = null;

            // Create a new TripleDES service provider 
            TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider {
                Key = pwdhash,
                Mode = CipherMode.ECB
            };

            encrypted = Convert.ToBase64String(
                tdesProvider.CreateEncryptor().TransformFinalBlock(inputBytes, 0, inputBytes.Length));
            return encrypted;
        }

        internal static string Decrypt(string encryptedString) {
            string decyprted = null;
            byte[] inputBytes = null;

            inputBytes = Convert.FromBase64String(encryptedString);
            byte[] pwdhash = null;

            //generate an MD5 hash from the password. 
            //a hash is a one way encryption meaning once you generate
            //the hash, you cant derive the password back from it.
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            pwdhash = hashmd5.ComputeHash(Encoding.ASCII.GetBytes(KEY));
            hashmd5 = null;

            // Create a new TripleDES service provider 
            TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider {
                Key = pwdhash,
                Mode = CipherMode.ECB
            };

            decyprted = Encoding.ASCII.GetString(
                tdesProvider.CreateDecryptor().TransformFinalBlock(inputBytes, 0, inputBytes.Length));
            return decyprted;
        }
    }

    /// <summary>
    /// Cookie helper
    /// </summary>
    internal static class CookieHelper {
        /// <summary>
        /// Gets the basic auth cookie.
        /// </summary>
        /// <returns></returns>
        internal static string GetBasicAuthCookie() {
            try {
                string cookieName = BasicAuthenticationHelper.CookieName;
                if(!string.IsNullOrEmpty(cookieName)) {
                    if(HttpContext.Current.Request.Cookies[cookieName] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies[cookieName].Value)) {
                        return
                            AuthenticationEncryption.Decrypt(
                                HttpContext.Current.Request.Cookies[cookieName].Value);
                    }
                }
            } catch(Exception ex) {
                ApplicationException aex = new ApplicationException("Crafted.Volva.CookieHelper.GetAuthCookie() threw an unhandled exception.", ex);
                //Logging.LogError(aex);
                return String.Empty;
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets the basic user.
        /// </summary>
        /// <returns></returns>
        internal static BasicUser GetBasicUser() {
            string value = GetBasicAuthCookie();
            if(!string.IsNullOrEmpty(value)) {
                XmlSerializer s = new XmlSerializer(typeof(BasicUser));
                return (BasicUser)s.Deserialize(new StringReader(value));
            }
            return null;
        }
        /// <summary>
        /// Sets the basic auth cookie.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="expires">The expires.</param>
        internal static void SetBasicAuthCookie(BasicUser user, int expires) {
            SetBasicAuthCookie(user, BasicAuthenticationHelper.CookieExpires);
        }

        /// <summary>
        /// Sets the basic auth cookie.
        /// </summary>
        /// <param name="user">The user.</param>
        internal static void SetBasicAuthCookie(BasicUser user) {
            if(user != null) {
                XmlSerializer s = new XmlSerializer(typeof(BasicUser));
                StringWriter sw = new StringWriter();
                s.Serialize(sw, user);
                string value = sw.ToString();
                SetBasicAuthCookie(value);
            }
        }

        /// <summary>
        /// Sets the basic auth cookie.
        /// </summary>
        /// <param name="value">The value.</param>
        internal static void SetBasicAuthCookie(string value) {
            SetBasicAuthCookie(value, BasicAuthenticationHelper.CookieExpires);
        }

        /// <summary>
        /// Sets the basic auth cookie.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="expires">The expires.</param>
        internal static void SetBasicAuthCookie(string value, int expires) {
            string cookieName = BasicAuthenticationHelper.CookieName;
            if(!string.IsNullOrEmpty(cookieName)) {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName] ?? new HttpCookie(cookieName);
                cookie.Value = AuthenticationEncryption.Encrypt(value);
                cookie.Expires = DateTime.Now.AddMinutes(expires);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// Removes the basic auth cookie.
        /// </summary>
        internal static void RemoveBasicAuthCookie() {
            string cookieName = BasicAuthenticationHelper.CookieName;
            if(!string.IsNullOrEmpty(cookieName)) {
                if(HttpContext.Current.Request.Cookies.AllKeys.Any(n => n == cookieName)) {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    cookie.Value = string.Empty;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }
    }
}
