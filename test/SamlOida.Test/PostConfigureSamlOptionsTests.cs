using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
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
        public void ShouldThrowOnMissingIdpCertificateValidatingSignatures()
        {
            var options = new SamlOptions
            {
                ServiceProviderEntityId = "MyId",
                AcceptSignedMessagesOnly = true,
                IdentityProviderCertificate = null
            };

            Assert.Throws<InvalidOperationException>(() => _target.PostConfigure(string.Empty, options));
        }

        [Fact]
        public void ShouldThrowOnMissingSpCertificateWhileSigningMessages()
        {
            var options = new SamlOptions
            {
                ServiceProviderEntityId = "MyId",
                SignOutgoingMessages = true,
                ServiceProviderCertificate = null
            };

            Assert.Throws<InvalidOperationException>(() => _target.PostConfigure(string.Empty, options));
        }

        [Fact]
        public void ShouldThrowOnMissingIdpSignonUrl()
        {
            var options = new SamlOptions
            {
                ServiceProviderEntityId = "MyId",
                IdentityProviderSignOnUrl = null
            };

            Assert.Throws<InvalidOperationException>(() => _target.PostConfigure(string.Empty, options));
        }
    }
}
