using System;
using System.Text.Json.Serialization;

namespace ffdc_sample_dotnet3.Models
{
    public class ClockServiceResponse
    {
        [JsonPropertyName("currentTime")]
        public DateTime CurrentTime { get; set; }
    }
}
