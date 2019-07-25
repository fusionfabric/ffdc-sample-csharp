using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using ffdc_client_credentials.Models;

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
        public string GenerateToken()
        {
            var clientApplication = new Dictionary<string, string>()
            {
                ["client_id"] = _configuration.GetValue<string>("finastra:oauth2:clientId"),
                ["client_secret"] = _configuration.GetValue<string>("finastra:oauth2:clientSecret"),
                ["grant_type"] = _configuration.GetValue<string>("finastra:oauth2:grantType")
            };

            FormUrlEncodedContent content = new FormUrlEncodedContent(clientApplication);

            HttpResponseMessage response = _httpClient.PostAsync(_configuration.GetValue<string>("finastra:oauth2:accessTokenEndpoint"), content).Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;

            JwtToken token = JsonConvert.DeserializeObject<JwtToken>(responseContent);
            
            return token?.access_token;            
        }

        /// <summary>
        /// Get the list of countries from FFDC referential data API by authenticating using jwt token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Countries GetCountries(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage refDataResponse = _httpClient.GetAsync("/referential/v1/countries").Result;
           
            string countryList = refDataResponse.Content.ReadAsStringAsync().Result;
            Countries result = JsonConvert.DeserializeObject<Countries>(countryList);
           
            return result;
        }
    }
}
