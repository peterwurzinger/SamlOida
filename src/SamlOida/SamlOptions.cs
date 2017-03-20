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

        public string ServiceProviderEntityId { get; set; }
        public TimeSpan IssueInstantExpiration { get; set; }
        public bool AcceptSignedAssertionsOnly { get; set; }

        public SamlBindingOptions SamlBindingOptions { get; set; }
    }

    public class SamlBindingOptions
    {
        public SamlBindingBehavior BindingBehavior { get; set; }
        public string IdentityProviderSignOnUrl { get; set; }
    }
}
