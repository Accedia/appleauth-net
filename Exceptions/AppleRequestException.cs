using System;

namespace AppleAuth.Exceptions
{
    /// <summary>
    /// Encapsulates error response from Apple's REST API
    /// </summary>
    public class AppleRequestException: Exception
    {
        internal AppleRequestException(string message) : base(message)
        {
            base.HelpLink = "https://developer.apple.com/documentation/sign_in_with_apple/errorresponse";
        }
    }
}
