using System.Text.Json.Serialization;

namespace AppleAuth.TokenObjects
{
    /// <summary>
    /// Received from Apple after successfull authentication of the user.
    /// </summary>
    public class AuthorizationToken
    {
        /// <summary>
        /// (Reserved for future use) A token used to access allowed data. 
        /// Currently, no data set has been defined for access.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AuthorizationCode { get; set; }
        /// <summary>
        /// Expiration time of the token in seconds.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresInSeconds { get; set; }
        /// <summary>
        /// A JSON Web Token that contains the user’s identity information.
        /// </summary>
        [JsonPropertyName("id_token")]
        public string Token { get; set; }        
        /// <summary>
        /// On success, the server issues a refresh token, which you use to validate if the user is still logged in using Apple ID.
        /// Store this token securely on your server.
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        /// <summary>
        /// The type of access token. It will always be 'Bearer'.
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Object containing information about the user. 
        /// </summary>
        /// <remarks>
        /// The user might choose to hide their email. In this case they use a private email with which you can
        /// communicate via the Private Email Relay Service. For more info visit: https://developer.apple.com/documentation/sign_in_with_apple/sign_in_with_apple_js/communicating_using_the_private_email_relay_service
        /// </remarks>
        public UserInformation UserInformation { get; set; }
    }
}
