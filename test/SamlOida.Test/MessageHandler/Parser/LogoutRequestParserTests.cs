using SamlOida.MessageHandler.Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Xunit;

namespace SamlOida.Test.MessageHandler.Parser
{
    public class LogoutRequestParserTests
    {
        private readonly LogoutRequestParser _logoutRequestParser;

        public LogoutRequestParserTests()
        {
            _logoutRequestParser = new LogoutRequestParser();
        }

        [Fact]
        public void ShouldParseMessage()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(
                "<samlp:LogoutRequest Destination='testDestination'" +
                                    " IssueInstant='2018-04-15T10:18:28.8732993Z'" +
                                    " Version='2.0' ID='Issuer_0afbe3ea-b930-49dc-ad84-6b3a9050caeb'" +
                                    " xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol'" +
                                    " xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>" +
                    "<saml:Issuer>testIssuer</saml:Issuer>" +
                    "<saml:NameID>testNameID</saml:NameID>" +
                    "<samlp:SessionIndex />" +
                 "</samlp:LogoutRequest>");

            var result = _logoutRequestParser.Parse(xmlDocument);

            Assert.Equal("Issuer_0afbe3ea-b930-49dc-ad84-6b3a9050caeb", result.Id);
            Assert.Equal("testDestination", result.Destination);
            Assert.Equal("testIssuer", result.Issuer);
            Assert.Equal("testNameID", result.NameId);
        }
    }
}
