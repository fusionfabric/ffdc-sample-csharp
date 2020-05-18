using System.Threading.Tasks;
using ffdc_sample_dotnet3.Core.Configuration;
using ffdc_sample_dotnet3.Core.HttpClients;
using ffdc_sample_dotnet3.Core.PrivateKeyTokenGenerator;
using IdentityModel;
using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ffdc_sample_dotnet3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddHttpContextAccessor();

            var finastraConfiguration = new FinastraConfiguration();
            var finastraConfigSection = Configuration.GetSection("Finastra");
            finastraConfigSection.Bind(finastraConfiguration);

            #region Configuration of Configuration -)
            services.Configure<FinastraConfiguration>(finastraConfigSection);
            services.AddSingleton<IValidateOptions<FinastraConfiguration>, ConfigurationValidator>();

            services.TryAddSingleton(provider => provider.GetRequiredService<IOptions<FinastraConfiguration>>().Value);
            #endregion

            // service providing private_key_jwt
            services.AddPrivateKeyJwtGenerator(o =>
                {
                    o.TokenEndpoint = finastraConfiguration.Oauth2Configuration.Issuer; // Finastra is validating against Issuer
                    o.TokenLifeTimeInMinutes =
                        finastraConfiguration.Oauth2Configuration.PrivateKeyJwtExpirationTimeInMinutes;
                }
            );

            #region B2B token management service configuration
            //for B2B private_key_jwt auth method - register custom client assertion configuration
            if (finastraConfiguration.Oauth2Configuration.ClientAuthenticationMethod ==
                ClientAuthenticationMethod.private_key_jwt)
            {
                services.AddTransient<ITokenClientConfigurationService, ClientAssertionTokenClientConfigurationService>();
            }// client_credentials will use DefaultTokenClientConfigurationService registered on next line 

            services.AddAccessTokenManagement(o =>
            {
                // Here we can register either one client token service or many, depending on configuration 
                // and business needs.
                // This sample registers one client named "finastra_login_service" per auth type.

                //in case of client_secret method - need to provide tokenEndpoint, clientId, secret.
                if (finastraConfiguration.Oauth2Configuration.ClientAuthenticationMethod ==
                    ClientAuthenticationMethod.client_secret)
                {
                    o.Client.Clients.Add("finastra_login_service", new ClientCredentialsTokenRequest
                    {
                        Address = finastraConfiguration.Oauth2Configuration.TokenEndpoint,
                        ClientId = finastraConfiguration.Oauth2Configuration.B2B.ClientId,
                        ClientSecret = finastraConfiguration.Oauth2Configuration.B2B.ClientSecret
                    });
                }
                else
                {
                    // this token client configured to use private_jwt method.
                    // in this case we need only tokenEndpoint - credentials set via private_key_jwt.
                    // ClientAssertion added in ClientAssertionTokenClientConfigurationService.GetClientCredentialsRequestAsync
                    // see class ClientAssertionTokenClientConfigurationService
                    o.Client.Clients.Add("finastra_login_service", new ClientCredentialsTokenRequest
                    {
                        Address = finastraConfiguration.Oauth2Configuration.TokenEndpoint
                    });
                }
            });

            // B2B flow config. Typed client registration and link to TokenManagementService. 
            services.AddHttpClient<IFfdcHttpClient, FfdcHttpClient>()
                .AddClientAccessTokenHandler("finastra_login_service");

            ////alternative way - use custom httpMessageHandler for outbound requests.
            ////This sample implementation can use service discovery, instead of explicit token, issuer addresses.
            //services.AddPrivateKeyJwtHttpClient(o =>
            //{
            //    o.AuthorityEndpoint = finastraConfiguration.Oauth2Configuration.AuthorityEndpoint;
            //});
            #endregion   


            #region B2C OIDC config start
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
               .AddCookie()
               .AddOpenIdConnect(o =>
               {
                   o.ClientId = finastraConfiguration.Oauth2Configuration.B2C.ClientId;
                   o.ClientSecret = finastraConfiguration.Oauth2Configuration.B2C.ClientSecret;
                   o.Authority = finastraConfiguration.Oauth2Configuration.AuthorityEndpoint;
                   o.CallbackPath = "/signin-oidc"; // this path or your custom one should be registered in FFDC application configuration. See FFDC application registration doc.
                   o.SaveTokens = true;
                   o.ResponseType = OpenIdConnectResponseType.Code;
                   // Indicates a Query Response see: http://openid.net/specs/openid-connect-core-1_0.html#ImplicitAuthResponse.
                   // other variant - form POST.
                   o.ResponseMode = OpenIdConnectResponseMode.Query;

                   o.Events.OnAuthorizationCodeReceived = context =>
                   {
                       if (finastraConfiguration.Oauth2Configuration.ClientAuthenticationMethod ==
                           ClientAuthenticationMethod.client_secret)
                       {
                           return Task.CompletedTask;
                           //for client secret no need to set clientAssertion.
                       }

                       var tokenGenerator = context.HttpContext.RequestServices.GetService<IPrivateKeyJwtGenerator>();
                       context.TokenEndpointRequest.ClientAssertionType = OidcConstants.ClientAssertionTypes.JwtBearer;
                       context.TokenEndpointRequest.ClientAssertion = tokenGenerator.CreateClientAuthJwt(new PrivateKeyJwtOptions
                       {
                           ClientId = finastraConfiguration.Oauth2Configuration.B2C.ClientId,
                           JwkKeyId = finastraConfiguration.Oauth2Configuration.B2B.JwkKeyId,
                           PrivateKeyPath = finastraConfiguration.Oauth2Configuration.B2B.PrivateKey
                           //other params if they are different in config
                       });
                       return Task.CompletedTask;
                   };
               });

            //B2C http client registration and attach userAccessTokenHandler
            //OAUTH configuration is taken from OpenIdConnect Config above
            services.AddHttpClient<IAccountsHttpClient, AccountsHttpClient>()
                .AddUserAccessTokenHandler();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }


    public class ClientAssertionTokenClientConfigurationService : DefaultTokenClientConfigurationService
    {
        private readonly IPrivateKeyJwtGenerator _privateKeyJwtGenerator;
        private readonly FinastraConfiguration _finastraConfiguration;

        public ClientAssertionTokenClientConfigurationService(IOptions<AccessTokenManagementOptions> accessTokenManagementOptions,
            IOptionsMonitor<OpenIdConnectOptions> oidcOptions,
            IAuthenticationSchemeProvider schemeProvider,
            IPrivateKeyJwtGenerator privateKeyJwtGenerator,
            FinastraConfiguration finastraConfiguration
        ) : base(accessTokenManagementOptions, oidcOptions, schemeProvider)
        {
            _privateKeyJwtGenerator = privateKeyJwtGenerator;
            _finastraConfiguration = finastraConfiguration;
        }

        // need this override to recalculate ClientAssertion.Value in B2B flow.
        public override async Task<ClientCredentialsTokenRequest> GetClientCredentialsRequestAsync(string clientName)
        {
            var r = await base.GetClientCredentialsRequestAsync(clientName);
            r.ClientAssertion.Type = OidcConstants.ClientAssertionTypes.JwtBearer;
            r.ClientAssertion.Value = _privateKeyJwtGenerator.CreateClientAuthJwt(new PrivateKeyJwtOptions
            {
                JwkKeyId = _finastraConfiguration.Oauth2Configuration.B2B.JwkKeyId,
                ClientId = _finastraConfiguration.Oauth2Configuration.B2B.ClientId,
                PrivateKeyPath = _finastraConfiguration.Oauth2Configuration.B2B.PrivateKey
            });
            return r;
        }
    }
}
