using SamlOida.MessageHandler.Parser;
using System;
using System.Linq;
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
        public void ShouldParseMessageWithoutAssertion()
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
        public void ShouldParseMessageWithAssertion()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(
                "<samlp:Response Destination='testDestination' IssueInstant='2018-04-08T12:57:54.7144887Z' Version='2.0' ID='Issuer_ff946305-a473-4c3e-a1f5-e9180a76dd07' xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol' xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>" +
                    "<saml:Issuer>testIssuer</saml:Issuer>" +
                    "<samlp:Status>" +
                        "<samlp:StatusCode Value='urn:oasis:names:tc:SAML:2.0:status:Success' />" +
                    "</samlp:Status> " +
                    "<saml:Assertion xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xs='http://www.w3.org/2001/XMLSchema' ID='_d71a3a8e9fcc45c9e9d248ef7049393fc8f04e5f75' Version='2.0' IssueInstant='2014-07-17T01:01:48Z'>" +
                        "<saml:Issuer>testAssertionIssuer</saml:Issuer>" +
                        "<saml:Subject>" +
                            "<saml:NameID SPNameQualifier='http://sp.example.com/demo1/metadata.php' Format='urn:oasis:names:tc:SAML:2.0:nameid-format:transient'>testSubjectID</saml:NameID>" +
                            "<saml:SubjectConfirmation Method='urn:oasis:names:tc:SAML:2.0:cm:bearer'>" +
                            "<saml:SubjectConfirmationData NotOnOrAfter='2024-01-18T06:21:48Z' Recipient='http://sp.example.com/demo1/index.php?acs' InResponseTo='ONELOGIN_4fee3b046395c4e751011e97f8900b5273d56685'/>" +
                            "</saml:SubjectConfirmation>" +
                        "</saml:Subject>" +
                        "<saml:Conditions NotBefore='2014-07-17T01:01:18Z' NotOnOrAfter='2024-01-18T06:21:48Z'>" +
                            "<saml:AudienceRestriction>" +
                            "<saml:Audience>http://sp.example.com/demo1/metadata.php</saml:Audience>" +
                            "</saml:AudienceRestriction>" +
                        "</saml:Conditions>" +
                        "<saml:AuthnStatement AuthnInstant='2014-07-17T01:01:48Z' SessionNotOnOrAfter='2024-07-17T09:01:48Z' SessionIndex='testSessionIndex'>" +
                            "<saml:AuthnContext>" +
                            "<saml:AuthnContextClassRef>urn:oasis:names:tc:SAML:2.0:ac:classes:Password</saml:AuthnContextClassRef>" +
                            "</saml:AuthnContext>" +
                        "</saml:AuthnStatement>" +
                        "<saml:AttributeStatement>" +
                            "<saml:Attribute Name='uid' NameFormat='urn:oasis:names:tc:SAML:2.0:attrname-format:basic'>" +
                            "<saml:AttributeValue xsi:type='xs:string'>test</saml:AttributeValue>" +
                            "</saml:Attribute>" +
                            "<saml:Attribute Name='mail' NameFormat='urn:oasis:names:tc:SAML:2.0:attrname-format:basic'>" +
                            "<saml:AttributeValue xsi:type='xs:string'>test@example.com</saml:AttributeValue>" +
                            "</saml:Attribute>" +
                        "</saml:AttributeStatement>" +
                    "</saml:Assertion>" +
                "</samlp:Response>"
            );

            var result = _authnResponseParser.Parse(xmlDocument);

            var issueInstant = DateTime.Parse("2018-04-08T12:57:54.7144887Z");

            Assert.True(result.IssueInstant.Equals(issueInstant));
            Assert.True(result.Success);
            Assert.False(result.Assertions.First().HasValidSignature);
            Assert.Equal("testAssertionIssuer", result.Assertions.First().Issuer);
            Assert.Equal("testSessionIndex", result.Assertions.First().SessionIndex);
            Assert.Equal("testSubjectID", result.Assertions.First().SubjectNameId);

            Assert.Equal("uid", result.Assertions.First().Attributes.First().Name);
            Assert.Equal("urn:oasis:names:tc:SAML:2.0:attrname-format:basic", result.Assertions.First().Attributes.First().NameFormat);
            Assert.Equal("test", result.Assertions.First().Attributes.First().Values.First());

            Assert.Equal("mail", result.Assertions.First().Attributes.Skip(1).First().Name);
            Assert.Equal("urn:oasis:names:tc:SAML:2.0:attrname-format:basic", result.Assertions.First().Attributes.Skip(1).First().NameFormat);
            Assert.Equal("test@example.com", result.Assertions.First().Attributes.Skip(1).First().Values.First());
        }

        [Fact]
        public void ShouldThrowParsingExceptionResponseMissing()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<foo />");

            var ex = Assert.Throws<ParsingException>(() => _authnResponseParser.Parse(xmlDocument));
            Assert.Equal("Element 'Response' missing.", ex.Message);
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
