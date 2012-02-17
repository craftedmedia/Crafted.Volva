using System.Configuration;
using Crafted.Configuration;
using Crafted.Configuration.Attributes;

namespace Crafted.Volva {

    /// <summary>
    /// Config for the basic authentication module
    /// </summary>
    [ConfigSection("Crafted.Volva")]
    internal class VolvaConfig : ConfigurationSection {
        /// <summary>
        /// Gets a value indicating whether [basic authentication is enabled].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [basic authentication is enabled]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("BasicAuthenticationIsEnabled", IsRequired = true, DefaultValue = false)]
        internal bool BasicAuthenticationIsEnabled {
            get {
                return (bool)this["BasicAuthenticationIsEnabled"];
            }
        }


        /// <summary>
        /// Gets the name of the cookie.
        /// </summary>
        /// <value>
        /// The name of the cookie.
        /// </value>
        [ConfigurationProperty("cookieName", IsRequired = false, DefaultValue = ".BASICAUTH")]
        internal string CookieName {
            get {
                return (string)this["cookieName"];
            }
        }

        /// <summary>
        /// Gets the cookie expires.
        /// </summary>
        [ConfigurationProperty("cookieExpires", IsRequired = false, DefaultValue = 60)]
        internal int CookieExpires {
            get {
                return (int)this["cookieExpires"];
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow windows users].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow windows users]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("allowWindowsUsers", IsRequired = false, DefaultValue = true)]
        public bool allowWindowsUsers {
            get {
                return (bool)this["allowWindowsUsers"];
            }
            set {
                this["allowWindowsUsers"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [ConfigurationProperty("message", IsRequired = false)]
        internal Message Message {
            get {
                return (Message)this["message"];
            }
            set {
                this["message"] = value;
            }
        }

        /// <summary>
        /// Gets the locations.
        /// </summary>
        [ConfigurationProperty("locations")]
        internal Locations Locations {
            get {
                return (Locations)this["locations"];
            }
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        [ConfigurationProperty("users")]
        internal Users Users {
            get {
                return (Users)this["users"];
            }
        }

    }

    /// <summary>
    /// Message Config element
    /// </summary>
    internal class Message : DefaultTextConfigElement {
    }

    /// <summary>
    /// List of locations
    /// </summary>
    [ConfigList("path")]
    internal class Locations : ConfigElementCollection<string, Path> {

    }

    /// <summary>
    /// Location element
    /// </summary>
    internal class Path : ConfigListElement<string> {

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public override string Key {
            get {
                return this.matchPattern;  
            }
            set {
                this.matchPattern = value;
            }
        }

        /// <summary>
        /// Gets or sets the match pattern for the location.
        /// </summary>
        /// <value>
        /// The match pattern.
        /// </value>
        [ConfigurationProperty("match")]
        public string matchPattern {
            get {
                return (string)this["match"]; 
            }
            set {
                this["match"] = value;
            }
        }
    }

    /// <summary>
    /// Collection of authenticated users
    /// </summary>
    [ConfigList("user")]
    internal class Users : ConfigElementCollection<string, User> {

    }

    /// <summary>
    /// User
    /// </summary>
    internal class User : ConfigListElement<string> {

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public override string Key {
            get {
                return this.Username;
            }
            set {
                this.Username = value;
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [ConfigurationProperty("username")]
        public string Username {
            get {
                return (string)this["username"];
            }
            set {
                this["username"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [ConfigurationProperty("password")]
        public string Password {
            get {
                return (string)this["password"];
            }
            set {
                this["password"] = value;
            }
        }
    }


}
