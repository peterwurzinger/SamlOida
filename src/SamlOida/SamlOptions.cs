using Microsoft.AspNetCore.Authentication;
using SamlOida.Binding;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using SamlOida.Model;

namespace SamlOida
{
    public class SamlOptions : RemoteAuthenticationOptions
    {
        public X509Certificate2 IdentityProviderCertificate { get; set; }
        public X509Certificate2 ServiceProviderCertificate { get; set; }
        public string ServiceProviderEntityId { get; set; }
        public string IdentityProviderSignOnUrl { get; set; }
        public string IdentityProviderLogOutUrl { get; set; }

        public Func<ICollection<SamlAttribute>, ICollection<Claim>> ClaimsSelector { get; set; }

        /// <summary>
        /// Used for Logout Requests/Responses
        /// </summary>
        public PathString LogoutPath { get; set; }
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
