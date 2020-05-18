using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ffdc_sample_dotnet3.Core.Configuration;
using ffdc_sample_dotnet3.Models;

namespace ffdc_sample_dotnet3.Core.HttpClients
{
    public class FfdcHttpClient : IFfdcHttpClient
    {
        private readonly HttpClient _httpClient;

        public FfdcHttpClient(HttpClient httpClient,
            FinastraConfiguration finastraConfiguration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(finastraConfiguration.ApiConfiguration.BaseUrl);
        }

        public async Task<ClockServiceResponse> GetUtcDateTimeAsync()
        {
            await using var responseStream = await _httpClient.GetStreamAsync("/sample/clock-service/v1/datetime");
            return await JsonSerializer.DeserializeAsync<ClockServiceResponse>(responseStream);
        }
    }
}
