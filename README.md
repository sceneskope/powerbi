# PowerBIClient
Dotnet core PowerBI client

A dotnet core client for using the PowerBI Rest API.
Designed to work with the latest Active Directory authentication that
does not support username and password.

The API is now generated from the swagger definitions!


## GetDeviceCode
Used this to get a device code for a client in order to allow headless
authentication.

## SceneSkope.PowerBI
The client and data models

## PowerBIClientTests
Unit(ish) tests that verify the API basically works

## Samples
Various samples showing how to use this


# Device codes
In order to use the API, device code authentication is used. See 
[https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-deviceprofile/](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-deviceprofile/)

The first thing to do is to register your app following the [instructions](https://dev.powerbi.com/apps?type=native).
Use "https://login.live.com/oauth20_desktop.srf" as the Redirect URL.

Typically you will only need the Dataset APIs enabled. Some of the tests need the group API enabled though.

When you select 'Register App' the Client ID will be generated. Take a copy of that!

Now run the GetDeviceCode console app from a command prompt.

    dotnet run <client id from above> <output file>

e.g.

    dotnet run xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx ..\PowerBIClientTesting.json

This then shows you how to obtain a device code. Follow the instructions and you should see Success displayed.

    Go to https://aka.ms/devicelogin and enter device code DZ7U3CRFS
    Success

The output file, in my case ..\PowerBIClientTesting.json will be produced containing the client id and the 
state of the token cache that can be used to create access tokens.










