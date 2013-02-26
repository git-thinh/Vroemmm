/*
 * Copyright (c) 2012 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
 * in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License
 * is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 * or implied. See the License for the specific language governing permissions and limitations under
 * the License.
 */

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
        public static readonly string CLIENT_ID = "7894937682-c7rgu3fs9ha0p7s0rl3t132uara5fpk0.apps.googleusercontent.com";

        /// <summary>
        /// The OAuth2.0 Client secret of your project.
        /// </summary>
        public static readonly string CLIENT_SECRET = "n3VZOg7UPzgSNu2bhKw-34CY";

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
        public static readonly string REDIRECT_URI = string.Format("{0}/GoogleAuthentication/", ConfigurationManager.AppSettings["baseurl"]);
    }
}
