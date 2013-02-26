using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using Google;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Calendar.v3;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Vroemmm.GoogleAuth
{
    public class Utils
    {
        /// <summary>
        /// Retrieve an IAuthenticator instance using the provided state.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials to use.</param>
        /// <returns>Authenticator using the provided OAuth 2.0 credentials</returns>
        public static IAuthenticator GetAuthenticatorFromState(IAuthorizationState credentials)
        {
            var provider = new StoredStateClient(GoogleAuthenticationServer.Description, ClientCredentials.CLIENT_ID, ClientCredentials.CLIENT_SECRET, credentials);
            var auth = new OAuth2Authenticator<StoredStateClient>(provider, StoredStateClient.GetState);
            auth.LoadAccessToken();
            return auth;
        }

        /// <summary>
        /// Retrieved stored credentials for the provided user ID.
        /// </summary>
        /// <param name="userId">User's ID.</param>
        /// <returns>Stored GoogleAccessProtectedResource if found, null otherwise.</returns>
        static IAuthorizationState GetStoredCredentials(String username)
        {
            var db = new Vroemmm.Models.VroemmmContext();
            StoredCredentials sc = db.StoredCredentialSet.FirstOrDefault(x => x.UserId == username);
            if (sc != null)
            {
                return new AuthorizationState() { AccessToken = sc.AccessToken, RefreshToken = sc.RefreshToken };
            }
            return null;
        }

        /// <summary>
        /// Store OAuth 2.0 credentials in the application's database.
        /// </summary>
        /// <param name="userId">User's ID.</param>
        /// <param name="credentials">The OAuth 2.0 credentials to store.</param>
        static void StoreCredentials(String username, IAuthorizationState credentials)
        {
            var db = new Vroemmm.Models.VroemmmContext();
            StoredCredentials sc = db.StoredCredentialSet.FirstOrDefault(x => x.UserId == username);
            if (sc != null)
            {
                sc.AccessToken = credentials.AccessToken;
                sc.RefreshToken = credentials.RefreshToken;
            }
            else
            {
                db.StoredCredentialSet.Add(new StoredCredentials { UserId = username, AccessToken = credentials.AccessToken, RefreshToken = credentials.RefreshToken });
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Exchange an authorization code for OAuth 2.0 credentials.
        /// </summary>
        /// <param name="authorizationCode">Authorization code to exchange for OAuth 2.0 credentials.</param>
        /// <returns>OAuth 2.0 credentials.</returns>
        /// <exception cref="CodeExchangeException">An error occurred.</exception>
        static IAuthorizationState ExchangeCode(String authorizationCode)
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, ClientCredentials.CLIENT_ID, ClientCredentials.CLIENT_SECRET);
            IAuthorizationState state = new AuthorizationState();
            state.Callback = new Uri(ClientCredentials.REDIRECT_URI);
            try
            {
                state = provider.ProcessUserAuthorization(authorizationCode, state);
                return state;
            }
            catch (Exception) //TODD: was ProtocolException, but this class occures in two dlls.
            {
                throw new CodeExchangeException(null);
            }
        }

        /// <summary>
        /// Send a request to the UserInfo API to retrieve the user's information.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials to authorize the request.</param>
        /// <returns>User's information.</returns>
        /// <exception cref="NoUserIdException">An error occurred.</exception>
        public static Userinfo GetUserInfo(IAuthenticator credentials) 
        {
            Oauth2Service userInfoService = new Oauth2Service(credentials);
            Userinfo userInfo = null;
            try {
                userInfo = userInfoService.Userinfo.Get().Fetch();
            } catch (GoogleApiRequestException e) {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            if (userInfo != null && !String.IsNullOrEmpty(userInfo.Id)) {
                return userInfo;
            } else {
                throw new NoUserIdException();
            }
        }

        /// <summary>
        /// Send a request to the UserInfo API to retrieve the user's information.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials to authorize the request.</param>
        /// <returns>User's information.</returns>
        /// <exception cref="NoUserIdException">An error occurred.</exception>
        public static Userinfo GetUserInfo(IAuthorizationState credentials)
        {
            return GetUserInfo(GetAuthenticatorFromState(credentials));
        }

        /// <summary>
        /// Retrieve the authorization URL.
        /// </summary>
        /// <param name="emailAddress">User's e-mail address.</param>
        /// <param name="state">State for the authorization URL.</param>
        /// <returns>Authorization URL to redirect the user to.</returns>
        public static String GetAuthorizationUrl(String emailAddress, String state)
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = ClientCredentials.CLIENT_ID;

            IAuthorizationState authorizationState = new AuthorizationState(ClientCredentials.SCOPES);
            authorizationState.Callback = new Uri(ClientCredentials.REDIRECT_URI);

            UriBuilder builder = new UriBuilder(provider.RequestUserAuthorization(authorizationState));
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(builder.Query);

            queryParameters.Set("access_type", "offline");
            queryParameters.Set("approval_prompt", "force");
            queryParameters.Set("user_id", emailAddress);
            queryParameters.Set("state", state);

            builder.Query = queryParameters.ToString();
            return builder.Uri.ToString();
        }

        /// <summary>
        /// Retrieve credentials using the provided authorization code.
        ///
        /// This function exchanges the authorization code for an access token and
        /// queries the UserInfo API to retrieve the user's e-mail address. If a
        /// refresh token has been retrieved along with an access token, it is stored
        /// in the application database using the user's e-mail address as key. If no
        /// refresh token has been retrieved, the function checks in the application
        /// database for one and returns it if found or throws a NoRefreshTokenException
        /// with the authorization URL to redirect the user to.
        /// </summary>
        /// <param name="authorizationCode">Authorization code to use to retrieve an access token.</param>
        /// <param name="state">State to set to the authorization URL in case of error.</param>
        /// <returns>OAuth 2.0 credentials instance containing an access and refresh token.</returns>
        /// <exception cref="CodeExchangeException">
        /// An error occurred while exchanging the authorization code.
        /// </exception>
        /// <exception cref="NoRefreshTokenException">
        /// No refresh token could be retrieved from the available sources.
        /// </exception>
        public static IAuthenticator GetCredentials(string username, String authorizationCode, String state)
        {
            String emailAddress = "";
            try
            {
                IAuthorizationState credentials = ExchangeCode(authorizationCode);

                if (string.IsNullOrEmpty(authorizationCode)) // no callback
                {
                    credentials = GetStoredCredentials(username);
                    if (credentials != null && !String.IsNullOrEmpty(credentials.RefreshToken))
                    {
                        return GetAuthenticatorFromState(credentials);
                    }
                }

                Userinfo userInfo = GetUserInfo(credentials);
                String userId = userInfo.Id;
                emailAddress = userInfo.Email;
                if (!String.IsNullOrEmpty(credentials.RefreshToken))
                {
                    StoreCredentials(emailAddress, credentials);
                    return GetAuthenticatorFromState(credentials);
                }
            }
            catch (CodeExchangeException e)
            {
                Console.WriteLine("An error occurred during code exchange.");
                // Drive apps should try to retrieve the user and credentials for the current
                // session.
                // If none is available, redirect the user to the authorization URL.
                e.AuthorizationUrl = GetAuthorizationUrl(emailAddress, state);
                throw e;
            }
            catch (NoUserIdException)
            {
                Console.WriteLine("No user ID could be retrieved.");
            }
            // No refresh token has been retrieved.
            String authorizationUrl = GetAuthorizationUrl(emailAddress, state);
            throw new NoRefreshTokenException(authorizationUrl);
        }

        /// <summary>
        /// Download a file and return a string with its content.
        /// </summary>
        /// <param name="auth">Authenticator responsible for creating web requests.</param>
        /// <param name="downloadUrl">Url to be used to download the resource.</param>
        public static string DownloadFile(IAuthenticator auth, String downloadUrl)
        {
            string result = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(downloadUrl));
                auth.ApplyAuthenticationToRequest(request);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                System.IO.Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            return result;
        }

        /// <summary>
        /// Build a Drive service object.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials.</param>
        /// <returns>Drive service object.</returns>
        internal static CalendarService BuildService(IAuthenticator credentials)
        {
            return new CalendarService(credentials);
        }
    }

    /// <summary>
    /// Exception thrown when an error occurred while retrieving credentials.
    /// </summary>
    public class GetCredentialsException : Exception
    {
        public String AuthorizationUrl { get; set; }

        /// <summary>
        /// Construct a GetCredentialsException.
        /// </summary>
        /// @param authorizationUrl The authorization URL to redirect the user to.
        public GetCredentialsException(String authorizationUrl)
        {
            this.AuthorizationUrl = authorizationUrl;
        }
    }

    /// <summary>
    /// Exception thrown when no authenticator has been found.
    /// </summary>
    public class NoAuthenticatorException : Exception
    {
    }

    /// <summary>
    /// Exception thrown when no refresh token has been found.
    /// </summary>
    public class NoRefreshTokenException : GetCredentialsException
    {
        /// <summary>
        /// Construct a NoRefreshTokenException.
        /// </summary>
        /// @param authorizationUrl The authorization URL to redirect the user to.
        public NoRefreshTokenException(String authorizationUrl)
            : base(authorizationUrl)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a code exchange has failed.
    /// </summary>
    public class CodeExchangeException : GetCredentialsException
    {
        /// <summary>
        /// Construct a CodeExchangeException.
        /// </summary>
        /// @param authorizationUrl The authorization URL to redirect the user to.
        public CodeExchangeException(String authorizationUrl) : base(authorizationUrl)
        {
        }
    }

    /// <summary>
    /// Exception thrown when no user ID could be retrieved.
    /// </summary>
    public class NoUserIdException : Exception
    {
    }

    /// <summary>
    /// Extends the NativeApplicationClient class to allow setting of a custom IAuthorizationState.
    /// </summary>
    public class StoredStateClient : NativeApplicationClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredStateClient"/> class.
        /// </summary>
        /// <param name="authorizationServer">The token issuer.</param>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        public StoredStateClient(AuthorizationServerDescription authorizationServer,
            String clientIdentifier,
            String clientSecret,
            IAuthorizationState state)
            : base(authorizationServer, clientIdentifier, clientSecret)
        {
            this.State = state;
        }

        public IAuthorizationState State { get; private set; }

        /// <summary>
        /// Returns the IAuthorizationState stored in the StoredStateClient instance.
        /// </summary>
        /// <param name="provider">OAuth2 client.</param>
        /// <returns>The stored authorization state.</returns>
        static public IAuthorizationState GetState(StoredStateClient provider)
        {
            return provider.State;
        }
    }
}
