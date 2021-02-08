using AppleAuth.Constants;
using AppleAuth.Cryptography;
using AppleAuth.Exceptions;
using AppleAuth.Http;
using AppleAuth.TokenObjects;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppleAuth
{
    /// <summary>
    ///<c>AppleAuthProvider</c> contains methods for retrieving an authorization token from Apple, 
    ///refreshing an existing token, generating `href` attribute for Sign In With Apple button
    /// </summary>
    /// <remarks>
    /// The tokens returned from Apple authorization servers are short lived and should be used for creating
    /// sessions or accounts in your system.
    /// </remarks> 
    public class AppleAuthProvider
    {
        private static string ClientID { get; set; }
        private static string TeamID { get; set; }
        private static string KeyID { get; set; }
        private static string RedirectURL { get; set; }
        private string State { get; set; }

        private static readonly AppleRestClient _appleRestClient = new AppleRestClient();
        private readonly TokenGenerator _tokenGenerator = new TokenGenerator();

        /// <summary>Constructor which initializes new instance of AppleAuthProvider with parameters used in the requests to Apple</summary>
        /// <param name="clientId">A10-character key identifier obtained from your developer account. (aka "Service ID" that is configured for “Sign In with Apple”)</param>
        /// <param name="teamId">A 10-character key identifier obtained from your developer account.</param>
        /// <param name="keyId">A 10-character key identifier obtained from your developer account. Configured for "Sign In with Apple" </param>
        /// <param name="redirectUrl">URL to which the user will be redirected after successful verification. 
        /// You need to configure a verified domain and map the redirect URL to it. Can’t be an IP address or localhost </param>
        /// <param name="state">Can be used for any internal identifiers (e.g. Session IDs, User IDs, Query Strings, etc.)</param>
        public AppleAuthProvider(string clientId, string teamId, string keyId, string redirectUrl, string state)
        {
            ClientID = clientId;
            TeamID = teamId;
            KeyID = keyId;
            RedirectURL = redirectUrl;
            State = state;
        }

        /// <summary>
        /// Retrieves an <c>AuthorizationToken</c> object from Apple. Use this object to create users or sessions.
        /// </summary>
        /// <exception cref="AppleRequestException">
        /// Thrown when HTTP response from Apple is different than 200 OK.
        /// </exception>
        /// <returns>
        /// AuthorizationToken object
        /// </returns>
        /// <params>
        /// <param name="authorizationCode">Received from Apple after successfully redirecting the user</param>
        /// <param name="privateKey">Content of the .p8 key file.</param>
        /// </params>
        /// <remarks>
        /// For more information about Apple's error responses visit: https://developer.apple.com/documentation/sign_in_with_apple/errorresponse
        /// </remarks>
        public async Task<AuthorizationToken> GetAuthorizationToken(string authorizationCode, string privateKey)
        {
            ValidateStringParameters(new List<string> { authorizationCode, privateKey });

            string clientSecret = _tokenGenerator.GenerateAppleClientSecret(privateKey, TeamID, ClientID, KeyID);

            HttpRequestMessage request = _appleRestClient.GenerateRequestMessage(TokenType.AuthorizationCode, authorizationCode, clientSecret, ClientID, RedirectURL);

            string response = await _appleRestClient.SentTokenRequest(request);

            var tokenResponse = JsonSerializer.Deserialize<AuthorizationToken>(response);

            TokenGenerator.VerifyAppleIDToken(tokenResponse.Token, ClientID);

            SetUserInformation(tokenResponse);

            return tokenResponse;
        }

        /// <summary>
        /// Verifies if a token is valid. 
        /// Use this method to check daily if the user is still signed in on your app using Apple ID.
        /// </summary>
        /// <exception cref="AppleRequestException">
        /// Thrown when HTTP response from Apple is different than 200 OK.
        /// </exception>
        ///<remarks>
        /// For reference of Apple's error responses visit: https://developer.apple.com/documentation/sign_in_with_apple/errorresponse
        /// </remarks>
        /// <param name="refreshToken">Received from Apple when successfully retrieving an AuthorizationToken object</param>
        /// <param name="privateKey">Content of the .p8 key file.</param>
        /// <returns>
        /// AuthorizationToken object containing only AuthorizationCode and Expiration
        /// </returns>
        public async Task<AuthorizationToken> GetRefreshToken(string refreshToken, string privateKey)
        {
            ValidateStringParameters(new List<string> { refreshToken, privateKey });
            string clientSecret = _tokenGenerator.GenerateAppleClientSecret(privateKey, TeamID, ClientID, KeyID);

            HttpRequestMessage request = _appleRestClient.GenerateRequestMessage(TokenType.RefreshToken, refreshToken, clientSecret, ClientID, RedirectURL);

            string response = await _appleRestClient.SentTokenRequest(request);

            var refreshTokenObject = JsonSerializer.Deserialize<AuthorizationToken>(response);

            return refreshTokenObject;
        }

        /// <summary>
        /// Generates url for the 'href' attribute of the "Sign in with Apple" button
        /// </summary>
        /// <remarks>
        /// For information how to display the "Sign in with Apple" buttons visit: https://developer.apple.com/documentation/sign_in_with_apple/sign_in_with_apple_js/displaying_sign_in_with_apple_buttons
        /// </remarks>
        /// <returns>string/url</returns>
        public string GetButtonHref()
        {
            var baseUrl = GlobalConstants.AppleAuthorizeURL;
            var requestParams = $"?client_id={ClientID}&scope=name email&redirect_uri={RedirectURL}&state={State}&response_type=code id_token&response_mode=form_post&usePopup=true";

            return baseUrl + requestParams;
        }

        /// <summary>
        /// Reads the JSON Web Token returned from apple 
        /// and creates new UserInformation object with information for the specific user
        /// </summary>
        /// <param name="tokenResponse">AuthorizationToken object</param>
        private void SetUserInformation(AuthorizationToken tokenResponse)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var deserializeUserInformation = tokenHandler.ReadJwtToken(tokenResponse.Token);

            if (deserializeUserInformation != null && deserializeUserInformation.Claims.Any())
            {
                var claims = deserializeUserInformation.Claims;

                var email = claims.FirstOrDefault(x => x.Type == ClaimConstants.Email);
                var email_verified = claims.FirstOrDefault(x => x.Type == ClaimConstants.EmailVerified);
                var sub = claims.FirstOrDefault(x => x.Type == ClaimConstants.Sub);
                var auth_time = claims.FirstOrDefault(x => x.Type == ClaimConstants.AuthenticationTime).Value;
                var timeOfAuthentication = DateTimeOffset.FromUnixTimeSeconds(long.Parse(auth_time)).DateTime;

                tokenResponse.UserInformation = new UserInformation
                {
                    Email = email != null ? email.Value : string.Empty,
                    EmailVerified = email_verified != null ? email_verified.Value : "False",
                    UserID = sub != null ? sub.Value : string.Empty,
                    TimeOfAuthentication = timeOfAuthentication
                };
            }
        }

        /// <summary>
        /// If any parameter is null or empty string throws an exception
        /// </summary>
        private void ValidateStringParameters(List<string> parameters)
        {
            if (parameters.Any(p => string.IsNullOrEmpty(p)))
            {
                throw new InvalidParametersException("One or more parameters is null");
            }
        }
    }
}
