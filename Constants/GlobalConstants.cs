namespace AppleAuth.Constants
{
    /// <summary>
    /// Globally used constants.
    /// </summary>
    internal static class GlobalConstants
    {
        internal const string BaseUri = "https://appleid.apple.com";
        internal const string AppleAuthorizeTokenURL = "/auth/token";
        internal const string AppleAuthorizeURL = "/auth/authorize";
        internal const string AppleAuthorizeRevokeURL = "/auth/revoke";
        internal const string ApplePublicKeysURL = "/auth/keys";
        internal static readonly string[] NewLineSeparators = { "\r", "\n", "\r\n" };
    }
}
