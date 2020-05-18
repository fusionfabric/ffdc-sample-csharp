using System.Text.Json.Serialization;

namespace ffdc_sample_dotnet3.Models
{
    public class AccountDetail
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
