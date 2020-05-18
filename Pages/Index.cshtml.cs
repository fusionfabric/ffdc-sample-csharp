using System.Threading.Tasks;
using ffdc_sample_dotnet3.Core.Configuration;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ffdc_sample_dotnet3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public FinastraConfiguration FinastraConfiguration { get; }

        public IndexModel(ILogger<IndexModel> logger,
       FinastraConfiguration configuration)
        {
            _logger = logger;
            FinastraConfiguration = configuration;
        }

        public async Task OnGetAsync()
        {

        }


    }
}
