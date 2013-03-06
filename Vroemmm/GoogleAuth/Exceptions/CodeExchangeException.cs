using System;

namespace Vroemmm.GoogleAuth
{
    /// <summary>
    /// Exception thrown when a code exchange has failed.
    /// </summary>
    public class CodeExchangeException : GetCredentialsException
    {
        /// <summary>
        /// Construct a CodeExchangeException.
        /// </summary>
        /// @param authorizationUrl The authorization URL to redirect the user to.
        public CodeExchangeException(String authorizationUrl)
            : base(authorizationUrl)
        {
        }
    }
}