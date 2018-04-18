using SamlOida.MessageHandler.Parser;
using System.Xml;
using Xunit;

namespace SamlOida.Test.MessageHandler.Parser
{
    public class LogoutRequestParserTests
    {
        private readonly LogoutRequestParser _logoutRequestParser;
        private readonly SamlOptions _options;

        public LogoutRequestParserTests()
        {
            _logoutRequestParser = new LogoutRequestParser();
            _options = new SamlOptions();
        }

        [Fact]
        public void ShouldParseMessage()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(
                "<samlp:LogoutRequest Destination='testDestination'" +
                                    " IssueInstant='2018-04-15T10:18:28.8732993Z'" +
                                    " Version='2.0'" +
                                    " ID='Issuer_0afbe3ea-b930-49dc-ad84-6b3a9050caeb'" +
                                    " xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol'" +
                                    " xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>" +
                    "<saml:Issuer>testIssuer</saml:Issuer>" +
                    "<saml:NameID>testNameID</saml:NameID>" +
                    "<samlp:SessionIndex />" +
                 "</samlp:LogoutRequest>");

            var result = _logoutRequestParser.Parse(xmlDocument, _options);

            Assert.Equal("Issuer_0afbe3ea-b930-49dc-ad84-6b3a9050caeb", result.Id);
            Assert.Equal("testDestination", result.Destination);
            Assert.Equal("testIssuer", result.Issuer);
            Assert.Equal("testNameID", result.NameId);
        }

        [Fact]
        public void ShouldThrowParsingExceptionLogoutRequestMissing()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<foo />");

            var ex = Assert.Throws<ParsingException>(() => _logoutRequestParser.Parse(xmlDocument, _options));
            Assert.Equal("Element 'LogoutRequest' missing.", ex.Message);
        }

        [Fact]
        public void ShouldThrowParsingExceptionNameIdMissing()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<samlp:LogoutRequest Destination='testDestination'" +
                                    " IssueInstant='2018-04-15T10:18:28.8732993Z'" +
                                    " Version='2.0' ID='Issuer_0afbe3ea-b930-49dc-ad84-6b3a9050caeb'" +
                                    " xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol'" +
                                    " xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>" +
                    "<saml:Issuer>testIssuer</saml:Issuer>" +
                    "<samlp:SessionIndex />" +
                 "</samlp:LogoutRequest>");

            var ex = Assert.Throws<ParsingException>(() => _logoutRequestParser.Parse(xmlDocument, _options));
            Assert.Equal("Node 'NameID' missing.", ex.Message);
        }
    }
}
