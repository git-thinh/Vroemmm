using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Calendar.v3;
using Google.Apis.Util;
using System;
using System.Diagnostics;


namespace Vroemmm.Logic
{
    public class AgendaImporter
    {
        public void ImportFromGoogle()
        {
            // Register the authenticator.
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = "7894937682-2vq2ag97174kjivhniemijaiv1f82nba.apps.googleusercontent.com";
            provider.ClientSecret = "5uwrq1-wnXMw8RfrPlcz8fFW";

            var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
            auth.LoadAccessToken();
            // Create the service.
            var service = new CalendarService(auth);
            
            var calenders = service.CalendarList.List().Fetch();
           
        }

        private static IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            // Get the auth URL:
            IAuthorizationState state = new AuthorizationState((new[] { CalendarService.Scopes.Calendar.GetStringValue() }));
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            Uri authUri = arg.RequestUserAuthorization(state);

            //Process.Start(authUri.ToString());
            //Console.Write("  Authorization Code: ");
            //string authCode = Console.ReadLine();
            //Console.WriteLine();

            var authCode = "4/dEI9atO-L397j9RoVvEeVDMflTnE.clwK8izsHyIZOl05ti8ZT3bpoi5teQI";
            // Retrieve the access token by using the authorization code:
            return arg.ProcessUserAuthorization(authCode, state);
        }
    }




}