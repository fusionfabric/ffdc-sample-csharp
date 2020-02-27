# Welcome

This sample client application is an implementation of the OAuth2 Authorization Code authorization grant for FusionFabric.cloud.

**To run this sample**
> You need a recent installation of .NET SDK. To find out more about it, and how to install .NET SDK, follow the [.NET Tutorial](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial/intro) for your operating system.

1. Register an application on [**Fusion**Fabric.cloud Developer Portal](https://developer.fusionfabric.cloud), and include the [Forex Spot Trade Capture API](https://developer.fusionfabric.cloud/api/trading-trade-capture-fx-spotd5339106-a27e-461f-a448-5480d222d460/docs). Use `https://localhost:4000/callback` as the reply URL.
2. Clone the current project.
3. Copy `src/appsettings.json.sample` to `src/appsettings.json`, open it, and enter `<%YOUR-CLIENT-ID%>`, and `<%YOUR-SECRET-KEY%>` of the application created at the step 1.  

> The `accessTokenEndpoint` and `authorizationEndpoint` are the access token and authorization endpoints provided by the [Discovery service](https://developer.fusionfabric.cloud/documentation/oauth2-grants#discovery-service) of the **Fusion**Fabric.cloud Developer Portal.

4. (Optional) If you want to use private key authentication, instead of the standard authentication based on secret value, follow [the steps from the documentation](https://developer.fusionfabric.cloud/ffdc-documentation/oauth2-grants.html#jwk-auth-procedure) to sign and upload a JSON Web Key to your application, and save the private RSA key stored in **src/Keys/private.pem**. Edit `appsettings.json` as follows:
+ Set `finastra.oauth2.strong` to `true`. 
+ Make sure `finastra.oauth2.KeyID` is set to the key ID - `kid` - of the JWK you uploaded to Developer Portal.
5. Open a Command Prompt or a Terminal in this directory and run the application with `dotnet run -p src\ffdc-authorization-code.csproj`. The application has started running. 
6. Point your browser to https://localhost:4000, and click **Signin**. You are redirected to the **Fusion**Fabric.cloud Developer Portal Authorization Server.
7. Log in with one of the following credentials:

| User        | Password |
| :---------- | :------- |
| `ffdcuser1` | `123456` |
| `ffdcuser2` | `123456` |

The home page of this sample application is displayed.

8. (Optional) Click **List Forex Spot Trades** in the home page to get the list of the Forex Spot trades, from the **Forex Spot for Trade Capture** API.
