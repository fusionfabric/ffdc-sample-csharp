using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.IO;
using ffdc_authorization_code.Models;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Jose;

namespace ffdc_authorization_code.Services
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
        /// Generates token using authorization code flow 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GenerateToken(string code, bool isStrong)
        {
            var clientApplication = new Dictionary<string, string>
            {
                ["client_id"] = _configuration.GetValue<string>("finastra:oauth2:clientId"),
                ["grant_type"] = _configuration.GetValue<string>("finastra:oauth2:grantType"),
                ["code"] = code,
                ["redirect_uri"] = _configuration.GetValue<string>("finastra:oauth2:redirectUri")

            };

            if (isStrong)
            {
                string token = CreateStrongToken();

                clientApplication.Add("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
                clientApplication.Add("client_assertion", token);
                FormUrlEncodedContent content = new FormUrlEncodedContent(clientApplication);

                HttpResponseMessage response = _httpClient.PostAsync(_configuration.GetValue<string>("finastra:oauth2:accessTokenEndpoint"), content).Result;
                string responseContent = response.Content.ReadAsStringAsync().Result;

                JwtToken jwtToken = JsonConvert.DeserializeObject<JwtToken>(responseContent);

                return jwtToken?.access_token;
            }
            else
            {
                clientApplication.Add("client_secret", _configuration.GetValue<string>("finastra:oauth2:clientSecret"));
                FormUrlEncodedContent content = new FormUrlEncodedContent(clientApplication);

                HttpResponseMessage response = _httpClient.PostAsync(_configuration.GetValue<string>("finastra:oauth2:accessTokenEndpoint"), content).Result;
                string responseContent = response.Content.ReadAsStringAsync().Result;

                JwtToken token = JsonConvert.DeserializeObject<JwtToken>(responseContent);

                return token?.access_token;
            }
        }

        /// <summary>
        /// Get the authorization code url of ffdc server
        /// </summary>
        /// <returns></returns>
        public string GetFFDCAuthCodeUri()
        {
            string clientId = _configuration.GetValue<string>("finastra:oauth2:clientId");
            string clientSecret = _configuration.GetValue<string>("finastra:oauth2:clientSecret");
            string grantType = _configuration.GetValue<string>("finastra:oauth2:grantType");
            string redirectUrl = _configuration.GetValue<string>("finastra:oauth2:redirectUri");

            string uriParams = $"client_id={clientId}&redirect_uri={redirectUrl}&scope=openapi&response_type=code&prompt=login";

            string url = $"{_httpClient.BaseAddress}{_configuration.GetValue<string>("finastra:oauth2:authorizationEndpoint")}?{uriParams}";

            return url;
        }

        /// <summary>
        /// Get list of trades from FFDC trade capture FX spot API by JWT token authentication
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public object GetResults(string token,out int responseCode)
        {
           _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage refDataResponse = _httpClient.GetAsync("/capital-market/trade-capture/static-data/v1/reference-sources?applicableEntities=legal-entities").Result;
            if (refDataResponse.IsSuccessStatusCode)
            {
                responseCode = (int)refDataResponse.StatusCode;
                string tradeSummary = refDataResponse.Content.ReadAsStringAsync().Result;

                TradeCaptureStaticDataList staticData = JsonConvert.DeserializeObject<TradeCaptureStaticDataList>(tradeSummary);
                return staticData;
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