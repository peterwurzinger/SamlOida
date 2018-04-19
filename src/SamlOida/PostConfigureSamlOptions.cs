using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;

namespace SamlOida
{
    public class PostConfigureSamlOptions : IPostConfigureOptions<SamlOptions>
    {
        private readonly ILogger<PostConfigureSamlOptions> _logger;

        public PostConfigureSamlOptions(ILogger<PostConfigureSamlOptions> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void PostConfigure(string name, SamlOptions options)
        {
            if (options.ServiceProviderEntityId == null)
                throw new ArgumentException("EntityID of Service Provider must be provided.");

            if (options.IdentityProviderCertificate != null && options.IdentityProviderCertificate.HasPrivateKey)
                _logger.LogWarning("Identity Provider Certificate contains a private key!");

            if (options.AcceptSignedMessagesOnly && options.IdentityProviderCertificate == null)
                throw new InvalidOperationException("Identity Provider Certificate needed for validating signatures of incoming messages. Set AcceptSignedResponsesOnly to false if signature validation is not intended.");

            if (options.SignOutgoingMessages && (options.ServiceProviderCertificate == null || !options.ServiceProviderCertificate.HasPrivateKey))
                throw new InvalidOperationException(
                    "Service Provider Certificate needed for signing outgoing messages. Set SignOutgoingMessages to false if signing is not intended.");

            if (options.IdentityProviderSignOnUrl == null)
                throw new InvalidOperationException("URL for SignOn needed");

            if (options.IdentityProviderLogOutUrl == null)
                _logger.LogWarning("SP initiated Single Logout won't be available due to lack of Sign-Out URL.");

            if (options.ClaimsSelector == null)
            {
                options.ClaimsSelector = _ => Array.Empty<Claim>();
                _logger.LogWarning("No conversion func between SAML-Attributes and System.Security.Claims.Claim supplied.");
            }

            if (options.CallbackPath == null)
                options.CallbackPath = "/saml-auth";

            if (options.LogoutPath == null)
                options.LogoutPath = "/saml-logout";

        }
    }
}
