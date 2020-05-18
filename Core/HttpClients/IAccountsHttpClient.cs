using System.Collections.Generic;
using System.Threading.Tasks;
using ffdc_sample_dotnet3.Models;

namespace ffdc_sample_dotnet3.Core.HttpClients
{
    public interface IAccountsHttpClient
    {
        Task<List<AccountSummary>> GetAccountSummariesAsync();
        Task<List<AccountDetail>> GetDetailsAsync(string accountId);
    }
}