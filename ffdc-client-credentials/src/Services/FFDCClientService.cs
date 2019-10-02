using System;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using ffdc_client_credentials.Models;

using Jose;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace ffdc_client_credentials.Services
{
    public class FFDCClientService
    {
        private HttpClient _httpClient;
        private IConfiguration _configuration;

        public FFDCClientService(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("finastra:oauth2:baseUrl"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// This generates jwt token from FFDC login service
        /// </summary>
        /// <returns></returns>
        public string GenerateToken(bool isStrong)
        {
            var clientApplication = new Dictionary<string, string>()
            {
                ["client_id"] = _configuration.GetValue<string>("finastra:oauth2:clientId"),
                ["grant_type"] = _configuration.GetValue<string>("finastra:oauth2:grantType")
            };

            JwtToken jwtToken = new JwtToken();
            if (isStrong)
            {
                string token = CreateStrongToken();

                clientApplication.Add("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
                clientApplication.Add("client_assertion", token);

                FormUrlEncodedContent content = new FormUrlEncodedContent(clientApplication);

                HttpResponseMessage response = _httpClient.PostAsync(_configuration.GetValue<string>("finastra:oauth2:accessTokenEndpoint"), content).Result;
                string responseContent = response.Content.ReadAsStringAsync().Result;

                jwtToken = JsonConvert.DeserializeObject<JwtToken>(responseContent);


            }
            else
            {
                clientApplication.Add("client_secret", _configuration.GetValue<string>("finastra:oauth2:clientSecret"));
                FormUrlEncodedContent content = new FormUrlEncodedContent(clientApplication);

                HttpResponseMessage response = _httpClient.PostAsync(_configuration.GetValue<string>("finastra:oauth2:accessTokenEndpoint"), content).Result;
                string responseContent = response.Content.ReadAsStringAsync().Result;

                jwtToken = JsonConvert.DeserializeObject<JwtToken>(responseContent);

            }
            return jwtToken?.access_token;
        }

        /// <summary>
        /// Get the list of countries from FFDC referential data API by authenticating using jwt token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public object GetCountries(string token, out int responseCode)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage refDataResponse = _httpClient.GetAsync("/referential/v1/countries").Result;
            if (refDataResponse.IsSuccessStatusCode)
            {
                responseCode = (int)refDataResponse.StatusCode;
                string countryList = refDataResponse.Content.ReadAsStringAsync().Result;
                Countries result = JsonConvert.DeserializeObject<Countries>(countryList);
                return result;
            }
            else
            {
                responseCode = (int)refDataResponse.StatusCode;
                return refDataResponse.StatusCode;
            }
        }

        /// <summary>
        /// Read the config value strong
        /// If it is true then API uses JWK token for authentication
        /// </summary>
        /// <returns></returns>
        public bool GetIsStrongValue()
        {
            return _configuration.GetValue<bool>("finastra:oauth2:strong");
        }

        /// <summary>
        /// This creates JWT Token and signin using private key,so it can be
        /// used to generate access token via client assertion
        /// </summary>
        /// <returns></returns>
        public string CreateStrongToken()
        {
            string clientId = _configuration.GetValue<string>("finastra:oauth2:clientId");
            string baseLogin = _configuration.GetValue<string>("finastra:oauth2:baseLogin");
            string privateKey = File.ReadAllText("./Keys/private.pem");
            var rsa = new RSACryptoServiceProvider(1024);

            string keyID = _configuration.GetValue<string>("finastra:oauth2:KeyID");
            TimeSpan span = ((DateTime.Now).AddMinutes(30)) - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            AsymmetricCipherKeyPair keyPair;
            using (var sr = new StringReader(privateKey))
            {
                var pr = new PemReader(sr);
                keyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
            }
            var payLoad = new Dictionary<string, object>
            {
                {"jti",Guid.NewGuid().ToString() },
                { "iss", clientId},
                { "exp", span.TotalSeconds },
                { "aud", baseLogin},
                { "sub", clientId},
            };
            var header = new Dictionary<string, object>
            {
                { "kid", keyID},
                { "typ", "JWT"}
            };

            RSAParameters rsaParameters = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);
            rsa.ImportParameters(rsaParameters);

            string jwtToken = JWT.Encode(payLoad, rsa, JwsAlgorithm.RS256, header);

            return jwtToken;

        }
    }
}
