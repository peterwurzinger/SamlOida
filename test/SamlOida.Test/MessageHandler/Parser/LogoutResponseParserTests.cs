using SamlOida.MessageHandler.Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Xunit;

namespace SamlOida.Test.MessageHandler.Parser
{
    public class LogoutResponseParserTests
    {
        private readonly LogoutResponseParser _logoutResponseParser;

        public LogoutResponseParserTests()
        {
            _logoutResponseParser = new LogoutResponseParser();
        }

        [Fact]
        public void ShouldParseSuccessMessage()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(
                "<samlp:LogoutResponse Destination='testDestination' " +
                                     " IssueInstant='2018-04-13T10:01:00.2665144Z'" +
                                     " Version='2.0'" +
                                     " ID='Issuer_0d404404-2ae3-4d7a-954a-957b74c32ed9'" +
                                     " xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol'" +
                                     " xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>" +
                    "<saml:Issuer>testIssuer</saml:Issuer>" +
                    "<samlp:Status>" +
                        "<samlp:StatusCode Value='urn:oasis:names:tc:SAML:2.0:status:Success'/>" +
                    "</samlp:Status>" +
                 "</samlp:LogoutResponse>"
            );

            var result = _logoutResponseParser.Parse(xmlDocument);

            Assert.Equal("Issuer_0d404404-2ae3-4d7a-954a-957b74c32ed9", result.Id);
            Assert.Equal("testDestination", result.Destination);
            Assert.Equal("testIssuer", result.Issuer);
            Assert.True(result.Success);
        }

        [Fact]
        public void ShouldParseResponderMessage()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(
                "<samlp:LogoutResponse Destination='testDestination' " +
                                     " IssueInstant='2018-04-13T10:01:00.2665144Z'" +
                                     " Version='2.0'" +
                                     " ID='Issuer_0d404404-2ae3-4d7a-954a-957b74c32ed9'" +
                                     " xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol'" +
                                     " xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>" +
                    "<saml:Issuer>testIssuer</saml:Issuer>" +
                    "<samlp:Status>" +
                        "<samlp:StatusCode Value='urn:oasis:names:tc:SAML:2.0:status:Responder'/>" +
                    "</samlp:Status>" +
                 "</samlp:LogoutResponse>"
            );

            var result = _logoutResponseParser.Parse(xmlDocument);

            Assert.Equal("Issuer_0d404404-2ae3-4d7a-954a-957b74c32ed9", result.Id);
            Assert.Equal("testDestination", result.Destination);
            Assert.Equal("testIssuer", result.Issuer);
            Assert.False(result.Success);
        }

        [Fact]
        public void ShouldThrowParsingExceptionLogoutRequestMissing()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<foo />");

            var ex = Assert.Throws<ParsingException>(() => _logoutResponseParser.Parse(xmlDocument));
            Assert.Equal("Element 'LogoutResponse' missing.", ex.Message);
        }

        [Fact]
        public void ShouldThrowParsingExceptionNameIDMissing()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(
                "<samlp:LogoutResponse Destination='testDestination' " +
                                     " IssueInstant='2018-04-13T10:01:00.2665144Z'" +
                                     " Version='2.0'" +
                                     " ID='Issuer_0d404404-2ae3-4d7a-954a-957b74c32ed9'" +
                                     " xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol'" +
                                     " xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>" +
                    "<saml:Issuer/>" +
                    "<samlp:Status>" +
                    "</samlp:Status>" +
                 "</samlp:LogoutResponse>"
            );

            var ex = Assert.Throws<ParsingException>(() => _logoutResponseParser.Parse(xmlDocument));
            Assert.Equal("Node 'StatusCode' missing.", ex.Message);
        }
    }
}
