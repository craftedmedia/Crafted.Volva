using System;
using System.Text;
using System.Web;

namespace Crafted.Volva {
    /// <summary>
    /// Basic Authentication Module
    /// </summary>
    public class BasicAuthentication : IHttpModule {

        HttpApplication _app;

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context) {

            this._app = context;

            if(BasicAuthenticationHelper.EnableBasicAuthentication) {
                _app.AuthorizeRequest += this.OnAuthorize;
                _app.BeginRequest += this.OnBeginRequest;
            }
        }

        /// <summary>
        /// Sends the auth challenge header.
        /// </summary>
        /// <param name="context">The context.</param>
        private void SendAuthChallengeHeader(HttpApplication context) {
            context.Response.Clear();
            context.Response.StatusCode = 401;
            context.Response.StatusDescription = "Unauthorized";
            context.Response.AddHeader("WWW-Authenticate", "Basic realm=\"" + BasicAuthenticationHelper.Message + "\"");
            context.Response.Write("401 Unauthorized");
            context.Response.End();
        }

        /// <summary>
        /// Sends the not authorized header.
        /// </summary>
        /// <param name="context">The context.</param>
        private void SendNotAuthorizedHeader(HttpApplication context) {
            context.Response.Clear();
            context.Response.StatusCode = 403;
            context.Response.StatusDescription = "Forbidden";
            context.Response.Write("403 Forbidden");
            context.Response.End();
        }

        /// <summary>
        /// Determines whether the specified context is authenticated.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if the specified context is authenticated; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAuthenticated(HttpApplication context) {
            string authHeader = context.Request.Headers["Authorization"];

            if(!string.IsNullOrEmpty(authHeader)) {

                if(authHeader.StartsWith("basic ", StringComparison.InvariantCultureIgnoreCase)) {
                    string userNameAndPassword = Encoding.Default.GetString(

                    Convert.FromBase64String(authHeader.Substring(6)));

                    string[] parts = userNameAndPassword.Split(':');

                    BasicUser bu = new BasicUser();
                    bu.UserName = parts[0];
                    bu.Password = parts[1];

                    if(BasicAuthenticationHelper.Authenticate(bu.UserName, bu.Password)) {
                        CookieHelper.SetBasicAuthCookie(bu);
                        return true;
                    } else {
                        if(!string.IsNullOrEmpty(CookieHelper.GetBasicAuthCookie())) {
                            CookieHelper.RemoveBasicAuthCookie();
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Called when [begin request].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnBeginRequest(object sender, EventArgs e) {
            HttpApplication context = sender as HttpApplication;

            if(string.IsNullOrEmpty(CookieHelper.GetBasicAuthCookie()) && BasicAuthenticationHelper.RequiresAuthentication(context.Request.Path)) {
                if(!IsAuthenticated(context)) {
                    SendAuthChallengeHeader(context);
                }
            }
        }

        /// <summary>
        /// Called when [authorize].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnAuthorize(object sender, EventArgs e) {
            HttpApplication context = sender as HttpApplication;

            if(BasicAuthenticationHelper.RequiresAuthentication(context.Request.Path)) {

                BasicUser bu = CookieHelper.GetBasicUser();
            
                if(bu == null || !(BasicAuthenticationHelper.Authenticate(bu.UserName, bu.Password))) {
                    SendNotAuthorizedHeader(context);
                }
            }
        }


        #region IHttpModule Members

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose() {

        }

        #endregion
    }
}
