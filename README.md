# SamlOida

[![AppVeyor](https://ci.appveyor.com/api/projects/status/c05vv9y58tbbcj1n/branch/master?svg=true)](https://ci.appveyor.com/project/peterwurzinger/samloida/branch/master)
[![Line coverage](https://samloida.blob.core.windows.net/samloida/report/badge_branchcoverage.svg)](https://samloida.blob.core.windows.net/samloida/report/index.htm)
[![Branch coverage](https://samloida.blob.core.windows.net/samloida/report/badge_linecoverage.svg)](https://samloida.blob.core.windows.net/samloida/report/index.htm)

A ASP.NET Core 2.0 Middelware to allow SAML authentication - supports single sign-out.

## Disclaimer

This application was built for academical purposes only. Please take this into consideration in terms of security decisions.

## Installation

#### via dotnet

`dotnet add package SamlOida --source https://www.myget.org/F/samloida/api/v3/index.json`

#### via nutget.exe

`nuget.exe install SamlOida -Source https://www.myget.org/F/samloida/api/v3/index.json`

## Example Usage

```csharp
public void ConfigureServices(IServiceCollection services) {
  services
    .AddAuthentication(options => {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = SamlAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignOutScheme = SamlAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options => {
    })
    .AddSaml(options => {
      	options.IssueInstantExpiration = TimeSpan.FromMinutes(20);
        options.ServiceProviderEntityId = "SamlOida";
        options.IdentityProviderSignOnUrl = "your-identity-provider-sign-on-url";
        options.CallbackPath = "your-sign-on-url";
        options.IdentityProviderLogOutUrl = "your-identity-provider-log-out-url";
        options.LogoutPath = "your-logout-url";
    })
}
```

## API

### `AddSaml`

### `options`

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our contributing process.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
