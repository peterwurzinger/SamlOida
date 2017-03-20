using System;
using Microsoft.AspNetCore.Builder;

namespace SamlOida
{
    public class SamlOptions : RemoteAuthenticationOptions, IResponseParsingOptions
    {
        public SamlOptions()
        {
            AuthenticationScheme = SamlDefaults.AuthenticationScheme;
            CallbackPath = "/signin-saml";
            IssueInstantExpiration = TimeSpan.FromMinutes(5);
            AcceptSignedAssertionsOnly = false;
        }

        public string Issuer { get; set; }
        public string LogOnUrl { get; set; }
        public TimeSpan IssueInstantExpiration { get; set; }
        public bool AcceptSignedAssertionsOnly { get; set; }
    }
}
