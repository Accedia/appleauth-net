using AppleAuth.Constants;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AppleAuth.Cryptography
{
    /// <summary>
    /// Class that manages generation of JWT tokens.
    /// </summary>
    public class TokenGenerator
    {
        private static readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Generates the signed JSON Web Token
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="teamId"></param>
        /// <param name="clientId"></param>
        /// <param name="keyId"></param>
        /// <param name="baseUri"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public string GenerateAppleClientSecret(string privateKey, string teamId, string clientId, string keyId, Uri baseUri, int expiration = 5)
        {
            var key = GetFormattedPrivateKey(privateKey);
            var ecDsaCng = ECDsa.Create();

            ecDsaCng.ImportPkcs8PrivateKey(Convert.FromBase64String(key), out var _);

            var signingCredentials = new SigningCredentials(
              new ECDsaSecurityKey(ecDsaCng), SecurityAlgorithms.EcdsaSha256);

            var now = DateTime.UtcNow;

            var expiresInMinutes = expiration > 5 ? now.AddMinutes(expiration) : now.AddMinutes(5);

            var claims = new List<Claim>
            {
                new Claim(ClaimConstants.Issuer, teamId),
                new Claim(ClaimConstants.IssuedAt, EpochTime.GetIntDate(now).ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimConstants.Expiration, EpochTime.GetIntDate(expiresInMinutes).ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimConstants.Audience, baseUri.AbsoluteUri),
                new Claim(ClaimConstants.Sub, clientId)
            };

            var token = new JwtSecurityToken(
                issuer: teamId,
                claims: claims,
                expires: expiresInMinutes,
                signingCredentials: signingCredentials);
             
            token.Header.Add(ClaimConstants.KeyID, keyId);

            return _tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Verify if the token that Apple has sent is valid and genuine.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="clientId"></param>
        /// <param name="baseUri"></param>
        public static void VerifyAppleIDToken(string token, string clientId, Uri baseUri)
        {
            var deserializedToken = _tokenHandler.ReadJwtToken(token);
            var claims = deserializedToken.Claims;

            SecurityKey publicKey;

            var expClaim = claims.FirstOrDefault(x => x.Type == ClaimConstants.Expiration).Value;
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).DateTime;

            if (expirationTime < DateTime.UtcNow)
            {
                throw new SecurityTokenExpiredException("Expired token");
            }

            var applePublicKeys = _httpClient.GetAsync(new Uri(baseUri, GlobalConstants.ApplePublicKeysURL));
            var keyset = new JsonWebKeySet(applePublicKeys.Result.Content.ReadAsStringAsync().Result);

            publicKey = keyset.Keys.FirstOrDefault(x => x.Kid == deserializedToken.Header.Kid);

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = baseUri.AbsoluteUri,
                IssuerSigningKey = publicKey,
                ValidAudience = clientId
            };

            _tokenHandler.ValidateToken(token, validationParameters, out var _);
        }

        /// <summary>
        /// Removes empty lines from string; Also removes lines like the following:"-----BEGIN PRIVATE KEY-----"
        /// </summary>
        public string GetFormattedPrivateKey(string keyString)
        {
            StringBuilder cleanedKey = new StringBuilder();
            var keyLines = keyString.Split(GlobalConstants.NewLineSeparators, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < keyLines.Length; i++)
            {
                if (!keyLines[i].Contains("PRIVATE"))
                {
                    cleanedKey.Append(keyLines[i]);
                }
            }

            return cleanedKey.ToString();
        }
    }
}
