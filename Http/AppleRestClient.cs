using System;
using AppleAuth.Constants;
using AppleAuth.Exceptions;
using AppleAuth.TokenObjects;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AppleAuth.Http
{
    /// <summary>
    /// Used for sending requests to Apple's REST services.
    /// </summary>
    internal class AppleRestClient
    {
        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Sends request to Apple's authorization endpoint.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal async Task<string> SendRequest(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode == false)
            {
                throw new AppleRequestException(content);
            }

            return content;
        }

        /// <summary>
        /// Generates an HttpRequestMessage as described in Apple's documentation: https://developer.apple.com/documentation/sign_in_with_apple/generate_and_validate_tokens
        /// </summary>
        internal HttpRequestMessage GenerateRequestMessage(string tokenType, string authorizationCode, string clientSecret, string clientId, string redirectUrl, Uri baseUri)
        {
            var bodyAsPairs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", tokenType),
                new KeyValuePair<string, string>("redirect_uri", redirectUrl)
            };

            if (tokenType == TokenType.RefreshToken)
                bodyAsPairs.Add(new KeyValuePair<string, string>("refresh_token", authorizationCode));
            else
                bodyAsPairs.Add(new KeyValuePair<string, string>("code", authorizationCode));

            var content = new FormUrlEncodedContent(bodyAsPairs);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return new HttpRequestMessage(HttpMethod.Post, new Uri(baseUri, GlobalConstants.AppleAuthorizeTokenURL)) { Content = content };
        }

        /// <summary>
        /// Generates an HttpRequestMessage for token revoke as described in Apple's documentation: https://developer.apple.com/documentation/sign_in_with_apple/revoke_tokens
        /// </summary>
        internal HttpRequestMessage GenerateRevokeMessage(string token, string clientSecret, string clientId, string tokenType, Uri baseUri)
        {
            var bodyAsPairs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("token", token)
            };

            if (!string.IsNullOrEmpty(tokenType))
            {
                bodyAsPairs.Add(new KeyValuePair<string, string>("token_type_hint", tokenType));
                
            }

            var content = new FormUrlEncodedContent(bodyAsPairs);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return new HttpRequestMessage(HttpMethod.Post, new Uri(baseUri, GlobalConstants.AppleAuthorizeRevokeURL)) { Content = content };
        }
    }
}
