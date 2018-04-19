using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;
using System.Xml;
using Xunit;

namespace SamlOida.Test.MessageHandler.MessageFactory
{
    public class LogoutRequestFactoryTests
    {
        private readonly LogoutRequestFactory _logoutRequestFactory;

        public LogoutRequestFactoryTests()
        {
            _logoutRequestFactory = new LogoutRequestFactory();
        }

        [Fact]
        public void ShouldCreateMessage()
        {
            var options = new SamlOptions {SignOutgoingMessages = false};

            var authnRequestMessage = new SamlLogoutRequestMessage {NameId = "test"};

            var xmlDocument = _logoutRequestFactory.CreateMessage(options, authnRequestMessage);

            var mgr = new XmlNamespaceManager(xmlDocument.NameTable);
            mgr.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            mgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

            var logoutResponseNode = xmlDocument.SelectSingleNode("/samlp:LogoutRequest", mgr);
            var issuerNode = xmlDocument.SelectSingleNode("/samlp:LogoutRequest/saml:Issuer", mgr);
            var nameIDNode = xmlDocument.SelectSingleNode("/samlp:LogoutRequest/saml:NameID", mgr);
            var sessionIndexNode = xmlDocument.SelectSingleNode("/samlp:LogoutRequest/samlp:SessionIndex", mgr);

            Assert.NotNull(logoutResponseNode);
            Assert.NotNull(issuerNode);
            Assert.NotNull(nameIDNode);
            Assert.NotNull(sessionIndexNode);

            Assert.Equal("", logoutResponseNode.Attributes["Destination"].Value);
            Assert.Equal("2.0", logoutResponseNode.Attributes["Version"].Value);

            Assert.Equal("test", nameIDNode.InnerText);

        }
    }
}
