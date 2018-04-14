using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;
using System;
using System.Collections.Generic;
using System.Text;
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

        /* TODO
        [Fact]
        public void ShouldCreateMessage()
        {
            var options = new SamlOptions();
            options.SignOutgoingMessages = false;

            var authnRequestMessage = new SamlLogoutRequestMessage();
            authnRequestMessage.NameId = "test";

            var xmlDocument = _logoutRequestFactory.CreateMessage(options, authnRequestMessage);

            // TODO
            // COPY PASTE FROM AuthnRequestFactory
            XmlNamespaceManager mgr = new XmlNamespaceManager(xmlDocument.NameTable);
            mgr.AddNamespjace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            mgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

            var authnRequestNode = xmlDocument.SelectSingleNode("/samlp:AuthnRequest", mgr);
            var issuerNode = xmlDocument.SelectSingleNode("/samlp:AuthnRequest/saml:Issuer", mgr);

            Assert.NotNull(authnRequestNode);
            Assert.NotNull(issuerNode);

            Assert.Equal("test", authnRequestNode.Attributes["AssertionConsumerServiceURL"].Value);
            Assert.Equal("", authnRequestNode.Attributes["Destination"].Value);
            Assert.Equal("2.0", authnRequestNode.Attributes["Version"].Value);
            
    }
    */
    }
}
