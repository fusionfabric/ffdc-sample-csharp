using System.Threading.Tasks;
using ffdc_sample_dotnet3.Core.Configuration;
using ffdc_sample_dotnet3.Core.HttpClients;
using ffdc_sample_dotnet3.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ffdc_sample_dotnet3.Pages
{
    public class UtcDateTimeModel : PageModel
    {
        public ClockServiceResponse ClockServiceResponse { get; set; }

        public string Token { get; set; }

        private readonly IFfdcHttpClient _ffdcHttpClient;
        public FinastraConfiguration FinastraConfiguration { get; }

        public UtcDateTimeModel(IFfdcHttpClient ffdcHttpClient, FinastraConfiguration finastraConfiguration)
        {
            _ffdcHttpClient = ffdcHttpClient;
            FinastraConfiguration = finastraConfiguration;
        }

        public async Task OnGet()
        {
            ClockServiceResponse = await _ffdcHttpClient.GetUtcDateTimeAsync();

            Token = await HttpContext.GetClientAccessTokenAsync();
        }

    }


}