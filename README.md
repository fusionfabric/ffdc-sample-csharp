# Welcome

This repository contains a sample client application, demonstrating the implementation of two OAuth2 
authorization grant flows for [FusionFabric.cloud](https://developer.fusionfabric.cloud/): 
 - `authorization code`, mandatory for `B2C`, and `B2E` [channel type APIs](https://developer.fusionfabric.cloud/documentation/creator-catalogs#api-channel-type);
 - `client credentials`, mandatory for `B2B`, and `SERVICE` channel type APIs.

> For more information about FusionFabric.cloud API offer, see the [documentation](https://developer.fusionfabric.cloud/documentation/creator-offer).

# How to use this repo

To run this sample you need a recent installation of .NET SDK. To find out more about it, and how to install .NET SDK, follow the [.NET Tutorial](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial/intro) for your operating system.

1. [Register an application](https://developer.fusionfabric.cloud/documentation/join-app-wizard) on FusionCreator, and include the following APIs:
   + [Account Information (US)](https://developer.fusionfabric.cloud/api/b2c-account-v1-fc77362a-c2ee-4b23-b20e-5621249eb7a4/docs)  - to work the Authorization Code grant flow.
   + [Clock Service](https://developer.fusionfabric.cloud/api/clock-v1-5ce28ddc-dbbc-11e9-9d36-2a2ae2dbcce4/docs) - to work the Client Credentials grant flow.
2. Clone the current project.
3. Copy `appsettings.json` to `appsettings.Developement.json`, open it, and enter the appropriate values for the following parameters:
   + `Finastra.Oauth2Configuration.B2B` and `Finastra.Oauth2Configuration.B2C`:
      + `ClientId` - the client ID of the corresponding API channel type.
      + `ClientSecret` - the secret key of the corresponding API channel type.  
   > You find these values in the application details page of the application that you created at step 1. For more information about how to generate the secrets, see [FusionCreator Documentation](https://developer.fusionfabric.cloud/documentation/creator-apps#credentials-by-api-channel).  
   + `Finastra.Oauth2Configuration.ClientAuthenticationMethod`: enter `client_secret` to enable the standard OAuth2 authorization flow.  For more details, see [FusionCreator Documentation](https://developer.fusionfabric.cloud/documentation/oauth2-grants).  
   > The `TokenEndpoint`, `AuthorityEndpoint`, and `Issuer` values provided by the [Discovery service](https://developer.fusionfabric.cloud/documentation/oauth2-grants#discovery-service) of FusionCreator.  
4. (Optional) If you want to use private key authentication, instead of the standard authentication based on secret value, follow the steps from the next section - [Private Key Configuration](#private-key-configuration), to sign and upload a JSON Web Key to your application, and save the private RSA key in **src/Keys/private.der**. Edit `appsettings.Development.json` as follows:
   + Set `Finastra.Oauth2Configuration.ClientAuthenticationMethod` to `private_key_jwt`. 
   + Make sure `JwkKeyId` of either `Finastra.Oauth2Configuration.B2B` or `Finastra.Oauth2Configuration.B2C`, or both, is set to the key ID - `kid` - of the JWK you uploaded to Developer Portal.
   > To read more about private key authentication on FusionCreator, see the [documentation](https://developer.fusionfabric.cloud/documentation/oauth2-grants#jwk-auth). 
5. Open a Command Prompt or a Terminal in this directory and run the following commands:
   ```
   dotnet build
   dotnet run
   ```  
6. Point your browser to https://localhost:5000. The homepage of sample application opens. The configuration for the home URL is stored in **Properties/launchSettings.json**.  
7. (Optional) Click one of the two buttons at the bottom of the cards: **Call B2B** or **Call B2C**.     
8. When you call the B2C API, you are prompted to authenticate with the Authorization Server of FusionFabric.cloud. Use one of the following credentials:

   | User        | Password |
   | :---------- | :------- |
   | `ffdcuser1` | `123456` |
   | `ffdcuser2` | `123456` |


### Private Key Configuration
You need to generate a pair of public and private SSL keys, and store the public key in a JWK that you upload to your application on FusionCreator. Follow the steps from [FusionCreator documentation](https://developer.fusionfabric.cloud/documentation/oauth2-grants#jwk-auth-procedure) to do so.

In addition to that, you must store the private key in the DER format, for compatibility with ASP.NET.

Run the following command:

```
openssl rsa -inform PEM -in private.key -outform DER -out private.der
```

**NOTES**:
> + Store your private key away from public storage, like source control.  
> + If you need to enable a stronger security, you can use password protected, encrypted private and public keys.


# License

This sample client application is released under the MIT License. See [LICENSE](LICENSE) for details.


![example workflow](https://github.com/fusionfabric/ffdc-sample-csharp/actions/workflows/superlinter.yml/badge.svg) [![FOSSA Status](https://app.fossa.com/api/projects/custom%2B24247%2Fgithub.com%2Ffusionfabric%2Fffdc-sample-csharp.svg?type=shield)](https://app.fossa.com/projects/custom%2B24247%2Fgithub.com%2Ffusionfabric%2Fffdc-sample-csharp?ref=badge_shield) [![PyPI license](https://img.shields.io/pypi/l/ansicolortags.svg)](https://pypi.python.org/pypi/ansicolortags/) [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)

