using SamlOida.MessageHandler.Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Xunit;

namespace SamlOida.Test.MessageHandler.Parser
{
    public class AuthnResponseParserTests
    {
        private readonly AuthnResponseParser _authnResponseParser;

        public AuthnResponseParserTests()
        {
            _authnResponseParser = new AuthnResponseParser();
        }

        //TODO: Check Parsing with Assertions

        [Fact]
        public void ShouldParseMessage()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(
                "<samlp:Response" +
                        " Destination='testDestination'" +
                        " IssueInstant='2018-04-08T12:57:54.7144887Z'" +
                        " Version='2.0'" +
                        " ID='Issuer_ff946305-a473-4c3e-a1f5-e9180a76dd07'" +
                        " xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol'" +
                        " xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>" +
                    "<saml:Issuer>testIssuer</saml:Issuer>" +
                    "<samlp:Status>" +
                        "<samlp:StatusCode Value='urn:oasis:names:tc:SAML:2.0:status:Success' />" +
                    "</samlp:Status> " +
                "</samlp:Response>"
            );

            var result = _authnResponseParser.Parse(xmlDocument);

            var issueInstant = DateTime.Parse("2018-04-08T12:57:54.7144887Z");

            Assert.True(result.IssueInstant.Equals(issueInstant));
            Assert.True(result.Success);
            Assert.Empty(result.Assertions);
        }

        [Fact]
        public void ShouldThrowParsingExceptionResponseMissing()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<foo />");

            var ex = Assert.Throws<ParsingException>(() => _authnResponseParser.Parse(xmlDocument));
            Assert.Equal("Element Response missing", ex.Message);
        }

        [Fact]
        public void ShouldThrowParsingExceptionStatusCodeMissing()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(
                "<samlp:Response" +
                        " Destination='testDestination'" +
                        " IssueInstant='2018-04-08T12:57:54.7144887Z'" +
                        " Version='2.0'" +
                        " ID='Issuer_ff946305-a473-4c3e-a1f5-e9180a76dd07'" +
                        " xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol'" +
                        " xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>" +
                    "<saml:Issuer>testIssuer</saml:Issuer>" +
                    "<samlp:Status>" +
                    "</samlp:Status> " +
                "</samlp:Response>"
            );

            var ex = Assert.Throws<ParsingException>(() => _authnResponseParser.Parse(xmlDocument));
            Assert.Equal("Element 'StatusCode' missing", ex.Message);
        }

        [Fact]
        public void ShouldThrowParsingExceptionIssueInstantMissing()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(
                "<samlp:Response" +
                        " Destination='testDestination'" +
                        " Version='2.0'" +
                        " ID='Issuer_ff946305-a473-4c3e-a1f5-e9180a76dd07'" +
                        " xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol'" +
                        " xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>" +
                    "<saml:Issuer>testIssuer</saml:Issuer>" +
                    "<samlp:Status>" +
                        "<samlp:StatusCode Value='urn:oasis:names:tc:SAML:2.0:status:Success' />" +
                    "</samlp:Status> " +
                "</samlp:Response>"
            );

            var ex = Assert.Throws<ParsingException>(() => _authnResponseParser.Parse(xmlDocument));
            Assert.Equal("Attribute 'IssueInstant' missing", ex.Message);
        }

        [Fact]
        public void ShouldThrowParsingExceptionIssueInstantInvalid()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(
                "<samlp:Response" +
                        " Destination='testDestination'" +
                        " IssueInstant='foo'" +
                        " Version='2.0'" +
                        " ID='Issuer_ff946305-a473-4c3e-a1f5-e9180a76dd07'" +
                        " xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol'" +
                        " xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>" +
                    "<saml:Issuer>testIssuer</saml:Issuer>" +
                    "<samlp:Status>" +
                        "<samlp:StatusCode Value='urn:oasis:names:tc:SAML:2.0:status:Success' />" +
                    "</samlp:Status> " +
                "</samlp:Response>"
            );

            var ex = Assert.Throws<ParsingException>(() => _authnResponseParser.Parse(xmlDocument));
            Assert.Equal("Issue instant cannot be parsed", ex.Message);
        }
    }
}
