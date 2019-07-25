using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

using ffdc_authorization_code.Models;

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
        public string GenerateToken(string code)
        {
            var clientApplication = new Dictionary<string, string>
            {
                ["client_id"] = _configuration.GetValue<string>("finastra:oauth2:clientId"),
                ["client_secret"] = _configuration.GetValue<string>("finastra:oauth2:clientSecret"),
                ["grant_type"] = _configuration.GetValue<string>("finastra:oauth2:grantType"),
                ["code"] = code,
                ["redirect_uri"] = _configuration.GetValue<string>("finastra:oauth2:redirectUri"),
                ["scope"] = "openid"
            };

            FormUrlEncodedContent content = new FormUrlEncodedContent(clientApplication);

            HttpResponseMessage response = _httpClient.PostAsync(_configuration.GetValue<string>("finastra:oauth2:accessTokenEndpoint"), content).Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;

            JwtToken token = JsonConvert.DeserializeObject<JwtToken>(responseContent);

            if (token == null || token.access_token == null)
            {
                return null;
            }
            return token.access_token;
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
        public FxSpotSummaryList GetFxSpotTrades(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage refDataResponse = _httpClient.GetAsync("/capital-market/trade-capture/fx/spot/v1/trades").Result;
            string tradeSummary = refDataResponse.Content.ReadAsStringAsync().Result;

            FxSpotSummaryList tradeSummaryList = JsonConvert.DeserializeObject<FxSpotSummaryList>(tradeSummary);
            return tradeSummaryList;
        }
    }
}