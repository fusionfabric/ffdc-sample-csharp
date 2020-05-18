namespace ffdc_sample_dotnet3.Core.PrivateKeyTokenGenerator
{
    public class PrivateKeyJwtOptions
    {
        public string PrivateKeyPath { get; set; }
        public string JwkKeyId { get; set; }
        public int TokenLifeTimeInMinutes { get; set; } = 1;
        public string ClientId { get; set; }
        public string TokenEndpoint { get; set; }
    }
}