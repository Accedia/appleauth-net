namespace AppleAuth.TokenObjects
{
    /// <summary>
    /// Can be used for mapping the body of the HTTP request on the specified Redirect URL.
    /// </summary>
    public class InitialTokenResponse
    {
        /// <summary>
        /// Can be used for any internal identifiers (e.g. Session IDs, User IDs, Query Strings, etc.).
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// Authorization code that should be exchanged for an Auhtorization token.
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// Initial JSON Web Token containing information about issuer, user, etc.
        /// </summary>
        public string id_token { get; set; }
        /// <summary>
        /// Information about the user. Send only the first time the user signs in.
        /// </summary>
        public string user { get; set; }
    }
}
