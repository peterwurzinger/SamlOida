using Microsoft.AspNetCore.Authentication;
using SamlOida.Binding;
using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;

namespace SamlOida
{
    public class SamlOptions : RemoteAuthenticationOptions
    {
        public X509Certificate2 IdentityProviderCertificate { get; set; }
        public X509Certificate2 ServiceProviderCertificate { get; set; }
        public string ServiceProviderEntityId { get; set; }
        public Uri IdentityProviderSignOnUrl { get; set; }
        public Uri IdentityProviderSignOutUrl { get; set; }

        /// <summary>
        /// Used for Logout Responses on SP initiated logouts
        /// </summary>
        public PathString SignoutCallbackPath { get; set; }

        /// <summary>
        /// Used for IdP initiated logouts
        /// </summary>
        public PathString SignoutPath { get; set; }
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
