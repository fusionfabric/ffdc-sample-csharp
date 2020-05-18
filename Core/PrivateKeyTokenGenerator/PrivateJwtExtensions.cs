using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ffdc_sample_dotnet3.Core.PrivateKeyTokenGenerator
{
    public static class PrivateKeyJwtExtensions
    {
        public static void AddPrivateKeyJwtGenerator(this IServiceCollection services, Action<PrivateKeyJwtOptions> options)
        {
            if (options != null)
            {
                services.Configure(options);
            }

            services.TryAddSingleton<IPrivateKeyJwtGenerator, PrivateKeyJwtGenerator>();
        }
    }
}
