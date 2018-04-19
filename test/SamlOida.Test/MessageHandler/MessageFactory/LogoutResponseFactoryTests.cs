using System;
using System.Xml;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;
using Xunit;

namespace SamlOida.Test.MessageHandler.MessageFactory
{
    public class LogoutResponseFactoryTests
    {
        private readonly LogoutResponseFactory _logoutResponseFactory;

        public LogoutResponseFactoryTests()
        {
            _logoutResponseFactory = new LogoutResponseFactory();
        }

        [Fact]
        public void ShouldCreateSuccessMessage()
        {
            var options = new SamlOptions {SignOutgoingMessages = false};

            var samlLogoutResponseMessage = new SamlLogoutResponseMessage
            {
                Success = true,
                InResponseTo = $"_{Guid.NewGuid():N}"
            };

            var xmlDocument = _logoutResponseFactory.CreateMessage(options, samlLogoutResponseMessage);

            XmlNamespaceManager mgr = new XmlNamespaceManager(xmlDocument.NameTable);
            mgr.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            mgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

            var logoutResponseNode = xmlDocument.SelectSingleNode("/samlp:LogoutResponse", mgr);
            var issuerNode = xmlDocument.SelectSingleNode("/samlp:LogoutResponse/saml:Issuer", mgr);
            var statusNode = xmlDocument.SelectSingleNode("/samlp:LogoutResponse/samlp:Status", mgr);
            var statusCodeNode = xmlDocument.SelectSingleNode("/samlp:LogoutResponse/samlp:Status/samlp:StatusCode", mgr);
            
            Assert.NotNull(logoutResponseNode);
            Assert.NotNull(issuerNode);
            Assert.NotNull(statusNode);
            Assert.NotNull(statusCodeNode);

            Assert.Equal("", logoutResponseNode.Attributes["Destination"].Value);
            Assert.Equal("2.0", logoutResponseNode.Attributes["Version"].Value);
            Assert.Equal(samlLogoutResponseMessage.InResponseTo, logoutResponseNode.Attributes["InResponseTo"]?.Value);

            Assert.Equal("urn:oasis:names:tc:SAML:2.0:status:Success", statusCodeNode.Attributes["Value"].Value);
        }

        [Fact]
        public void ShouldCreateFailureMessage()
        {
            var options = new SamlOptions {SignOutgoingMessages = false};

            var samlLogoutResponseMessage = new SamlLogoutResponseMessage {Success = false};

            var xmlDocument = _logoutResponseFactory.CreateMessage(options, samlLogoutResponseMessage);

            var mgr = new XmlNamespaceManager(xmlDocument.NameTable);
            mgr.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            mgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

            var logoutResponseNode = xmlDocument.SelectSingleNode("/samlp:LogoutResponse", mgr);
            var issuerNode = xmlDocument.SelectSingleNode("/samlp:LogoutResponse/saml:Issuer", mgr);
            var statusNode = xmlDocument.SelectSingleNode("/samlp:LogoutResponse/samlp:Status", mgr);
            var statusCodeNode = xmlDocument.SelectSingleNode("/samlp:LogoutResponse/samlp:Status/samlp:StatusCode", mgr);

            Assert.NotNull(logoutResponseNode);
            Assert.NotNull(issuerNode);
            Assert.NotNull(statusNode);
            Assert.NotNull(statusCodeNode);

            Assert.Equal("", logoutResponseNode.Attributes["Destination"].Value);
            Assert.Equal("2.0", logoutResponseNode.Attributes["Version"].Value);

            Assert.Equal("urn:oasis:names:tc:SAML:2.0:status:Responder", statusCodeNode.Attributes["Value"].Value);
        }
    }
}
