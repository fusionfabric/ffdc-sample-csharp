using System.Text.Json.Serialization;

namespace ffdc_sample_dotnet3.Models
{
    public class AccountSummary
    {
        [JsonPropertyName("accountId")]
        public string AccountId { get; set; }

        [JsonPropertyName("accountType")]
        public string AccountType { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("formattedAccountNumber")]
        public string formattedAccountNumber { get; set; }
    }
}
