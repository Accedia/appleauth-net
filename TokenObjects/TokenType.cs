namespace AppleAuth.TokenObjects
{
    /// <summary>
    /// Sets the value of "grant_type" when maing a request to Apple's authentication endpoint.
    /// </summary>
    public static class TokenType
    {
        /// <summary>
        /// Used for retrieving a JSON Web Token that contains the user’s identity information.
        /// </summary>
        public const string AuthorizationCode = "authorization_code";
        /// <summary>
        /// Used for verifying if a token is valid.
        /// </summary>
        public const string RefreshToken = "refresh_token";
    }
}
