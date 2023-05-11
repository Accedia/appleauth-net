[![NuGet](https://img.shields.io/badge/nuget-v1.0.6-blue)](https://www.nuget.org/packages/AppleAuth.NET/1.0.6)

# What is AppleAuth.NET?
AppleAuth is a very simple library for .NET that encapsulates the logic for communicating with [Apple's REST API for Sign in with Apple](https://developer.apple.com/documentation/sign_in_with_apple/sign_in_with_apple_rest_api).
The main goal is to make the implementation of [Sign in with Apple](https://developer.apple.com/sign-in-with-apple/) easier for any web application.

# How to use it?
## Installation
To install the package execute the following command in your Package Manager Console:
```javascript
PM> Install-Package AppleAuth.NET
```
Or alternatively just install the package using Nuget package manager. The project can be found here:
[Link to NuGet](https://www.nuget.org/packages/AppleAuth.NET)

# Prerequisites
## Configure Sign in with Apple from the Developer Portal
In order to use Sign in with Apple you must enroll in the [Apple Developer Program](https://developer.apple.com/programs/enroll/).
After you have enrolled in the program go to [Developer Account Help](https://help.apple.com/developer-account/) and navigate to Configure app capabilities > Sign in with Apple.
There you can find the information for configuring Sign in with Apple for your app.

You can also checkout my [blogpost](https://accedia.com/blog/dotnetifying-sign-in-with-apple/) for more information on setting the settings in your developer account implementing Sign in with Apple.

## Display the "Sign in with Apple" button
Next, you have to configure your web page for Sign in with Apple. Follow the guidelines from the official [documentation](https://developer.apple.com/documentation/sign_in_with_apple/sign_in_with_apple_js/configuring_your_webpage_for_sign_in_with_apple). You can also refer to this [link](https://developer.apple.com/documentation/sign_in_with_apple/sign_in_with_apple_js/displaying_sign_in_with_apple_buttons) to see how to setup the styles of the buttons.

## Configure your hosting environment
If you are deploying your app to an Azure Web App make sure you add the following setting: `WEBSITE_LOAD_USER_PROFILE = 1`, so IIS can access the private key storage under the user account store.
You can apply this from the Azure portal from Configuration > Application Settings, or you can run the following command in Cloud Shell:   
``` az webapp config appsettings set --name <app-name> --resource-group <resource-group-name> --settings WEBSITE_LOAD_USER_PROFILE=1 ```.  
It's also important to note that this setting is available only for non-shared pricing tiers.

If you are deploying your app to your own webserver, running Microsoft IIS, you'll need to enable "Load User Profile" under "Advanced settings" on your Application Pool. Otherwise you'll get a CryptographicException saying "The system cannot find the file specified".
# Example

## Using ```AppleAuthProvider.cs```
Create new instance of `AppleAuthProvider`, pass the required parameters and you are good to go. Use the `GetAuthorizationToken` method to get an authorization token from Apple; Use the `GetRefreshToken` method to verify if a user is still using 'Sign in with Apple' to sign in your system; Use the `GetButtonHref` method to get a query string for the 'Sign in with Apple' button.

## Handling initial response from Apple
After the user clicks on the "Sign in with Apple" button on your page they will be redirected to https://appleid.apple.com/. After they provide their credentials Apple will make a POST request to the url that you have specified as Redirect URL. You can handle the request using ```InitialTokenResponse.cs```. In order to retrieve an authorization token you should first create new instance of `AppleAuthProvider` with the required parameters.
After that just call GetAuthorizationToken() method passing ```code``` from your ```InitialTokenResponse``` object and your private key. Here is a sample implementation in C#:

```c#
        [HttpPost]
        public async Task HandleResponseFromApple(AppleAuth.TokenObjects.InitialTokenResponse response)
        {
            string privateKey = System.IO.File.ReadAllText("path/to/file.p8");

            AppleAuth.AppleAuthProvider provider = new AppleAuth.AppleAuthProvider("MyClientID", "MyTeamID", "MyKeyID", "MyRedirectUrl", "SomeState");

            AppleAuth.TokenObjects.AuthorizationToken authorizationToken = await provider.GetAuthorizationToken(response.code, privateKey);
        }
```

Keep in mind that tokens returned from Apple are short-lived, so you should create a session or a user in your system using the returned ```AppleAuth.TokenObjects.AuthorizationToken ``` object. After that you can verify if the user is still logged in using "Sign in with Apple" by retrieving a refresh token using the ```GetRefreshToken``` method:

```c#
        [HttpPost]
        public async Task<bool> IsUserUsingAppleID()
        {
            string privateKey = System.IO.File.ReadAllText("path/to/file.p8");

            AppleAuth.AppleAuthProvider provider = new AppleAuthProvider("MyClientID", "MyTeamID", "MyKeyID", "https://myredirecturl.com/HandleResponseFromApple", "SomeState");

            AppleAuth.TokenObjects.AuthorizationToken refreshToken = await provider.GetRefreshToken(authorizationToken.RefreshToken, privateKey);

            return refreshToken != null;
        }
```
# Contributing

You are more than welcome to contribute to the project and make it better. When contributing please try to maintain a strictly professional, respectful and friendly attitude. Also make sure you communicate the change you want to make via issue or any other method with the owners of this repository.

### **Creating a pull request**
We do not have any strict guidelines for creating pull requests, but you can use the already known [GitHub flow]("https://git-scm.com/book/en/v2/GitHub-Contributing-to-a-Project") for general guidelines.


# License
This project is licensed under the MIT License - see [LICENSE.md](https://github.com/Accedia/appleauth-net/blob/master/LICENSE.md) for details
