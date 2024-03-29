﻿namespace AppleAuth.Constants
{
    /// <summary>
    /// JSON Web Token Claims constants.
    /// </summary>
    internal static class ClaimConstants
    {
        internal const string Issuer = "iss";
        internal const string IssuedAt = "iat";
        internal const string Expiration = "exp";
        internal const string Audience = "aud";
        internal const string Sub = "sub";
        internal const string KeyID = "kid";

        internal const string Email = "email";
        internal const string EmailVerified = "email_verified";
        internal const string AuthenticationTime = "auth_time";
        internal const string Nonce = "nonce";
    }
}
