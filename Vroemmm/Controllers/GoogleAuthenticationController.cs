using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Calendar.v3;
using Google.Apis.Util;
using Google.Apis.Authentication;
using Vroemmm.GoogleAuth;

namespace Vroemmm.Controllers
{
    public partial class GoogleAuthenticationController : Controller
    {
        public virtual ActionResult Authenticate(string state, string code, string callbackurl)
        {
            if (!string.IsNullOrEmpty(callbackurl))
            {
                Session["callbackUrl"] = callbackurl;
            }

            try
            {
                IAuthenticator authenticator = Utils.GetCredentials(User.Identity.Name, code, state);
                // Store the authenticator and the authorized service in session
                Session["authenticator"] = authenticator;
            }
            catch (CodeExchangeException)
            {
                if (Session["service"] == null || Session["authenticator"] == null)
                {
                    return Redirect(Utils.GetAuthorizationUrl("", state));
                }
            }
            catch (NoRefreshTokenException e)
            {
                return Redirect(e.AuthorizationUrl);
            }

            if (string.IsNullOrEmpty(callbackurl))
            {
                if (Session["callbackUrl"] != null)
                {
                    callbackurl = Session["callbackUrl"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(callbackurl))
            {
                return Redirect(callbackurl);
            }

            return RedirectToRoute("default");
        }
    }
}
