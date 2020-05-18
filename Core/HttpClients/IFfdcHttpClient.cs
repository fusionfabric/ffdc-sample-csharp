using System.Threading.Tasks;
using ffdc_sample_dotnet3.Models;

namespace ffdc_sample_dotnet3.Core.HttpClients
{
    public interface IFfdcHttpClient
    {
        Task<ClockServiceResponse> GetUtcDateTimeAsync();
    }
}