using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace SamlOida.Test
{
    public class PostConfigureSamlOptionsTests
    {
        private readonly PostConfigureSamlOptions _target;

        public PostConfigureSamlOptionsTests()
        {
            _target = new PostConfigureSamlOptions(new Logger<PostConfigureSamlOptions>(new NullLoggerFactory()));
        }

        [Fact]
        public void ShouldThrowWhenLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PostConfigureSamlOptions(null));
        }

        [Fact]
        public void ShouldThrowOnMissingServiceProviderEntityId()
        {
            var options = new SamlOptions
            {
                ServiceProviderEntityId = null,
            };

            Assert.Throws<ArgumentException>(() => _target.PostConfigure(string.Empty, options));
        }

        [Fact]
        public void ShouldThrowOnMissingIdentityProviderCertificate()
        {
            var options = new SamlOptions
            {
                ServiceProviderEntityId = "MyId",
                AcceptSignedMessagesOnly = true,
                IdentityProviderCertificate = null,
            };

            var ex = Assert.Throws<InvalidOperationException>(() => _target.PostConfigure(string.Empty, options));
            Assert.Equal("Identity Provider Certificate needed for validating signatures of incoming messages. Set AcceptSignedResponsesOnly to false if signature validation is not intended.", ex.Message);
        }

        [Fact]
        public void ShouldThrowOnMissingServiceProviderCertificate()
        {
            var options = new SamlOptions
            {
                ServiceProviderEntityId = "MyId",
                ServiceProviderCertificate = null,
                AcceptSignedMessagesOnly = false,
                IdentityProviderCertificate = null,
                SignOutgoingMessages = true
            };

            var ex = Assert.Throws<InvalidOperationException>(() => _target.PostConfigure(string.Empty, options));
            Assert.Equal("Service Provider Certificate needed for signing outgoing messages. Set SignOutgoingMessages to false if signing is not intended.", ex.Message);
        }

        [Fact]
        public void ShouldThrowOnMissingIdentityProviderSignOnUrl()
        {
            var options = new SamlOptions
            {
                ServiceProviderEntityId = "MyId",
                ServiceProviderCertificate = null,
                AcceptSignedMessagesOnly = false,
                IdentityProviderCertificate = null,
                SignOutgoingMessages = false,
                IdentityProviderSignOnUrl = null,
                IdentityProviderLogOutUrl = null,
            };

            var ex = Assert.Throws<InvalidOperationException>(() => _target.PostConfigure(string.Empty, options));
            Assert.Equal("URL for SignOn needed", ex.Message);
        }

        [Fact]
        public void ShouldSetClaimsSelector()
        {
            var options = new SamlOptions
            {
                ServiceProviderEntityId = "MyId",
                ServiceProviderCertificate = null,
                AcceptSignedMessagesOnly = false,
                IdentityProviderCertificate = null,
                SignOutgoingMessages = false,
                IdentityProviderSignOnUrl = "ips-sign-on-url",
                IdentityProviderLogOutUrl = "ips-log-out-url",
                ClaimsSelector = null,
            };

            _target.PostConfigure(string.Empty, options);
            Assert.NotNull(options.ClaimsSelector);
        }

        [Fact]
        public void ShouldDefaultCallpackPathAndLogoutPath()
        {
            var options = new SamlOptions
            {
                ServiceProviderEntityId = "MyId",
                ServiceProviderCertificate = null,
                AcceptSignedMessagesOnly = false,
                AcceptSignedAssertionsOnly = false,
                IdentityProviderCertificate = null,
                SignOutgoingMessages = false,
                IdentityProviderSignOnUrl = "ips-sign-on-url",
                IdentityProviderLogOutUrl = "ips-log-out-url",
                CallbackPath = null,
                LogoutPath = null,
            };

            _target.PostConfigure(string.Empty, options);
            Assert.Equal("/saml-auth", options.CallbackPath);
            Assert.Equal("/saml-logout", options.LogoutPath);
        }
    }
}
