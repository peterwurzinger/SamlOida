# SamlOida

[![MyGet](https://img.shields.io/myget/samloida/v/samloida.svg)](https://www.myget.org/feed/samloida/package/nuget/SamlOida)
[![Downloads](https://img.shields.io/myget/samloida/dt/samloida.svg)](https://www.myget.org/feed/samloida/package/nuget/SamlOida)
[![AppVeyor](https://ci.appveyor.com/api/projects/status/c05vv9y58tbbcj1n/branch/master?svg=true)](https://ci.appveyor.com/project/peterwurzinger/samloida/branch/master)
[![Line coverage](https://samloida.blob.core.windows.net/samloida/report/badge_linecoverage.svg)](https://samloida.blob.core.windows.net/samloida/report/index.htm)
[![Branch coverage](https://samloida.blob.core.windows.net/samloida/report/badge_branchcoverage.svg)](https://samloida.blob.core.windows.net/samloida/report/index.htm)

A ASP.NET Core 2.0 Middelware to allow SAML authentication. It supports
* Single Sign-on
	* IdP initiated
	* SP initiated
* Single Sign-out
	* IdP initiated Single Sign-out
	* SP initiated Single Sign-out
* Signatures
	* Signing outgoing Requests & Responses
	* Validating signatures of incoming Requests & Responses
* Encryption
	* EncryptedAssertion
* Bindings
	* HTTP Redirect Binding w/ SAML Deflate Encoding
	* HTTP Post Binding 

## Disclaimer

This application was built for academical purposes only. If you need a production ready framework you might want to check out Anders Abel's [Sustainsys](https://github.com/Sustainsys/Saml2).
**Do not use the library in production environment unless you know exactly what you are doing!**

## Installation

#### via dotnet

`dotnet add package SamlOida --source https://www.myget.org/F/samloida/api/v3/index.json`

#### via nutget.exe

`nuget.exe install SamlOida -Source https://www.myget.org/F/samloida/api/v3/index.json`

## Example Usage

```csharp
public void ConfigureServices(IServiceCollection services) {
  var spCert = new X509Certificate2(File.ReadAllBytes("spPrivateCertificate.pfx"), PASSWORD);
  var idpCert = new X509Certificate2(File.ReadAllBytes("idpPublicCertificate.cer"));
  
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
        options.ServiceProviderEntityId = "your-entity-id";
        options.IdentityProviderSignOnUrl = "your-identity-provider-sign-on-url";
        options.IdentityProviderLogOutUrl = "your-identity-provider-log-out-url";
        options.CallbackPath = "your-sign-on-url";
        options.LogoutPath = "your-logout-url";
      
      	options.IssueInstantExpiration = TimeSpan.FromMinutes(20);

        options.AcceptSignedMessagesOnly = true;
        options.SignOutgoingMessages = true;
        options.AcceptSignedAssertionsOnly = false;
      
        options.ServiceProviderCertificate = spCert;
        options.IdentityProviderCertificate = idpCert;

        options.ClaimsSelector = (attributes) =>
        {
          return attributes.Select(attr => new Claim(attr.Name, attr.Values.FirstOrDefault()))
            .ToList();
		};
      
      	options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
}
```

## API

### `public static class SamlExtensions`

| Methods                                  |
| ---------------------------------------- |
| AddSaml(Action &lt;SamlOptions&gt;)      |
| AddSaml(string authenticationScheme, Action &lt;SamlOptions&gt; options) |
| AddSaml(string authenticationScheme, string displayName, Action&lt;SamlOptions&gt; options) |


### `public class SamlOptions `

`:  Microsoft.AspNetCore.Authentication.RemoteAuthenticationOptions`

| Property                    | Type                                     | DefaultValue     |
| --------------------------- | ---------------------------------------- | ---------------- |
| ServiceProviderEntityId     | string                                   | `null`           |
| IdentityProviderSignOnUrl   | string                                   | `null`           |
| IdentityProviderLogOutUrl   | string                                   | `null`           |
| CallbackPath                | string                                   | `"/saml-auth"`   |
| LogoutPath                  | string                                   | `"/saml-logout"` |
| IssueInstantExpiration      | TimeSpan                                 | `null`           |
| AcceptSignedMessagesOnly    | bool                                     | `true`           |
| SignOutgoingMessages        | bool                                     | `true`           |
| AcceptSignedAssertionsOnly  | bool                                     | `false`          |
| ServiceProviderCertificate  | X509Certificate2                         | `null`           |
| IdentityProviderCertificate | X509Certificate2                         | `null`           |
| LogoutResponseBinding       | SamlBindingBehavior                      | `null`           |
| LogoutRequestBinding        | SamlBindingBehavior                      | `null`           |
| AuthnRequestBinding         | SamlBindingBehavior                      | `null`           |
| ClaimsSelector              | Func &lt;ICollection&lt;SamlAttribute&gt;, ICollection&lt;Claim&gt;&gt; | `null`           |

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our contributing process.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.