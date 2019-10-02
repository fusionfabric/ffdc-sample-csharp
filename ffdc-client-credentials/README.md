# Welcome

This sample client application is an implementation of the OAuth2 Client Credentials authorization grant flow for FusionFabric.cloud.

**To run this sample**

> You need a recent installation of .NET SDK. To find out more about it, and how to install .NET SDK, follow the [.NET Tutorial](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial/intro) for your operating system.
 
1. Register an application on [**Fusion**Fabric.cloud Developer Portal](https://developer.fusionfabric.cloud), and include the **Referential Data** API. Use `*` as the reply URL.
2. Clone the current project.
3. Copy `src/appsettings.json.sample` to `src/appsettings.json`, open it and enter `<%YOUR-CLIENT-ID%>`, and `<%YOUR-SECRET-KEY%>` of the application created at the step 1.

> The  `accessTokenEndpoint` is the token endpoint provided by the [Discovery service](https://developer.fusionfabric.cloud/documentation?workspace=FusionCreator%20Developer%20Portal&board=Home&uri=oauth2-grants.html#discovery-service) of the **Fusion**Fabric.cloud Developer Portal.

4. (Optional) If you want to use private key authentication, instead of the standard authentication based on secret value, follow [the steps from the documentation](https://developer.fusionfabric.cloud/documentation?workspace=FusionCreator%20Developer%20Portal&board=Home&uri=oauth2-grants.html#jwk-auth-procedure) to sign and upload a JSON Web Key to your application, and save the private RSA key stored in **src/Keys/private.pem**. Edit `appsettings.json` as follows:
+ Set `finastra.oauth2.strong` to `true`. 
+ Make sure `finastra.oauth2.KEYID` is set to the key ID - `kid` - of the JWK you uploaded to Developer Portal.
5. Open a Command Prompt or a Terminal in this directory and run the application with `dotnet run -p src\ffdc-client-credentials.csproj`. The application has started running.
6. Point your browser to http://localhost:4000. The list of countries is retrieved from the **Referential Data** API.
