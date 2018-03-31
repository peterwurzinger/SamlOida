using Microsoft.AspNetCore.Authentication;
using SamlOida.Binding;
using System;
using System.Security.Cryptography.X509Certificates;

namespace SamlOida
{
    public class SamlOptions : RemoteAuthenticationOptions
    {
        public X509Certificate2 IdentityProviderCertificate { get; set; }
        public X509Certificate2 ServiceProviderCertificate { get; set; }
        public Uri IdentityProviderSignOnUrl { get; set; }
        public Uri IdentityProviderSignOutUrl { get; set; }
        public SamlBindingBehavior AuthnRequestBinding { get; set; }
        public SamlBindingBehavior LogoutRequestBinding { get; set; }
        public SamlBindingBehavior LogoutResponseBinding { get; set; }
        public TimeSpan IssueInstantExpiration { get; set; }
        public bool AcceptSignedAssertionsOnly { get; set; }
        public bool AcceptSignedMessagesOnly { get; set; }
        public bool SignOutgoingMessages { get; set; }

        public SamlOptions()
        {
            SignOutgoingMessages = true;
            AcceptSignedMessagesOnly = true;
        }

    }
}
