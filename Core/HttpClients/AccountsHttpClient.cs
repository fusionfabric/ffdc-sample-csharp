using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ffdc_sample_dotnet3.Core.Configuration;
using ffdc_sample_dotnet3.Models;

namespace ffdc_sample_dotnet3.Core.HttpClients
{
    public class AccountsHttpClient : IAccountsHttpClient
    {
        private readonly HttpClient _httpClient;

        public AccountsHttpClient(HttpClient httpClient,
            FinastraConfiguration finastraConfiguration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri($"{finastraConfiguration.ApiConfiguration.BaseUrl}/retail-us/me/account/v1/accounts/");
        }
        public async Task<List<AccountSummary>> GetAccountSummariesAsync()
        {
            var stream = await _httpClient.GetStreamAsync("");
            var data = await JsonSerializer.DeserializeAsync<List<AccountSummary>>(stream);
            return data;
        }

        public async Task<List<AccountDetail>> GetDetailsAsync(string accountId)
        {
            var stream = await _httpClient.GetStreamAsync($"{accountId}/details");
            var data = await JsonSerializer.DeserializeAsync<List<AccountDetail>>(stream);
            return data;
        }

    }
}
