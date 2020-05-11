namespace AppleAuth.Constants
{
    /// <summary>
    /// Globally used constants.
    /// </summary>
    internal static class GlobalConstants
    {
        internal const string AppleAuthorizeTokenURL = "https://appleid.apple.com/auth/token";
        internal const string AppleAuthorizeURL = "https://appleid.apple.com/auth/authorize";
        internal static readonly string[] NewLineSeparators = { "\r", "\n", "\r\n" };
    }
}
