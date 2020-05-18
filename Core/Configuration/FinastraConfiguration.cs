namespace ffdc_sample_dotnet3.Core.Configuration
{
    public class FinastraConfiguration
    {
        public FinastraConfiguration()
        {
            Oauth2Configuration = new Oauth2Configuration();
            ApiConfiguration = new ApiConfiguration();
        }
        public Oauth2Configuration Oauth2Configuration { get; set; }
        public ApiConfiguration ApiConfiguration { get; set; }
    }
}
