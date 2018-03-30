using Microsoft.AspNetCore.Authentication;
using SamlOida.Binding;
using System;
using System.Security.Cryptography.X509Certificates;

namespace SamlOida
{
    public class SamlOptions : RemoteAuthenticationOptions
    {
        public X509Certificate2 IdentityProviderCertificate { get; set; }

        //Warn on presence of private key
        public X509Certificate2 ServiceProviderCertificate { get; set; }
        public Uri IdentityProviderSignOnUrl { get; set; }

        public Uri IdentityProviderSignOutUrl { get; set; }
        public SamlBindingBehavior SingleSignOnBinding { get; set; }
        public TimeSpan IssueInstantExpiration { get; set; }

        public bool AcceptSignedAssertionsOnly { get; set; }

        public bool AcceptSignedResponsesOnly { get; set; }
        public bool EncryptResponse { get; set; }

        public bool SignRequest { get; set; }
        public bool SignResponse { get; set; }
    }
}
