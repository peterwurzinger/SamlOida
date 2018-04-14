using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Xunit;

namespace SamlOida.Test
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
            var options = new SamlOptions();
            options.SignOutgoingMessages = false;

            var samlLogoutResponseMessage = new SamlLogoutResponseMessage();
            samlLogoutResponseMessage.Success = true;

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

            Assert.Equal("urn:oasis:names:tc:SAML:2.0:status:Success", statusCodeNode.Attributes["Value"].Value);
        }

        [Fact]
        public void ShouldCreateResponderMessage()
        {
            var options = new SamlOptions();
            options.SignOutgoingMessages = false;

            var samlLogoutResponseMessage = new SamlLogoutResponseMessage();
            samlLogoutResponseMessage.Success = false;

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

            Assert.Equal("urn:oasis:names:tc:SAML:2.0:status:Responder", statusCodeNode.Attributes["Value"].Value);
        }
    }
}
