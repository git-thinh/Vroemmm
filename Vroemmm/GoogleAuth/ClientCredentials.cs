using Google.Apis.Calendar.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.Util;
using Google.Apis.Oauth2.v2.Data;
using System.Configuration;

namespace Vroemmm.GoogleAuth
{
    internal static class ClientCredentials
    {
        /// <summary>
        /// The OAuth2.0 Client ID of your project.
        /// </summary>
        public static readonly string CLIENT_ID = ConfigurationManager.AppSettings["GoogleClientId"];

        /// <summary>
        /// The OAuth2.0 Client secret of your project.
        /// </summary>
        public static readonly string CLIENT_SECRET = ConfigurationManager.AppSettings["GoogleClientSecret"];

        /// <summary>
        /// The OAuth2.0 scopes required by your project.
        /// </summary>
        public static readonly string[] SCOPES = new String[]
        {
            "https://www.googleapis.com/auth/userinfo.email",
            "https://www.googleapis.com/auth/userinfo.profile",
            CalendarService.Scopes.Calendar.GetStringValue()
        };

        /// <summary>
        /// The Redirect URI of your project.
        /// </summary>
        public static readonly string REDIRECT_URI = string.Format("{0}/GoogleAuthentication/", ConfigurationManager.AppSettings["BaseUrl"]);
    }
}
