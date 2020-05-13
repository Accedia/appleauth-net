# What is AppleAuth.NET?
AppleAuth is a very simple library for .NET that encapsulates the logic for communicating with [Apple's REST API for Sign in with Apple](https://developer.apple.com/documentation/sign_in_with_apple/sign_in_with_apple_rest_api).
The main goal is to make the implementation of [Sign in with Apple](https://developer.apple.com/sign-in-with-apple/) easier for any web application.

# How to use it?
## Installation
To install the package execute the following command in your Package Manager Console:
```javascript
PM> Install-Package AppleAuth
```
Or alternatively just install the package using Nuget package manager. The project can be found here:
[Link to NuGet](https://www.nuget.org/packages/AppleAuth.NET/1.0.0)

## Using ```AppleAuthProvider.cs```
Create new instance of `AppleAuthProvider`, pass the required parameters and you are good to go. Use the `GetAuthorizationToken` method to get an authorization token from Apple; Use the `GetRefreshToken` method to verify if a user is still using 'Sign in with Apple' to sign in your system; Use the `GetButtonHref` method to get a query string for the 'Sign in with Apple' button.

## Display the "Sign in with Apple" button
Next, you have to configure your web page for Sign in with Apple. Follow the guidelines from the official [documentation](https://developer.apple.com/documentation/sign_in_with_apple/sign_in_with_apple_js/configuring_your_webpage_for_sign_in_with_apple). You can also refer to this [link](https://developer.apple.com/documentation/sign_in_with_apple/sign_in_with_apple_js/displaying_sign_in_with_apple_buttons) to see how to setup the styles of the buttons.

# Prerequisites
## Configure Sign in with Apple from the Developer Portal
In order to use Sign in with Apple you must enroll in the [Apple Developer Program](https://developer.apple.com/programs/enroll/).
After you have enrolled in the program go to [Developer Account Help](https://help.apple.com/developer-account/) and navigate to Configure app capabilities > Sign in with Apple.
There you can find the information for configuring Sign in with Apple for your app.
