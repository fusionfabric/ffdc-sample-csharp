namespace ffdc_sample_dotnet3.Core.PrivateKeyTokenGenerator
{
    public interface IPrivateKeyJwtGenerator
    {
        string CreateClientAuthJwt();
        string CreateClientAuthJwt(PrivateKeyJwtOptions options);
    }
}