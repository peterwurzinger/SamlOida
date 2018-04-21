using SamlOida.MessageHandler.Parser;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Xunit;

namespace SamlOida.Test.MessageHandler.Parser
{
    public class AuthnResponseParserTests
    {
        private readonly AuthnResponseParser _authnResponseParser;
        private readonly SamlOptions _options;

        public AuthnResponseParserTests()
        {
            _authnResponseParser = new AuthnResponseParser();
            _options = new SamlOptions();
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

            var result = _authnResponseParser.Parse(xmlDocument, _options);

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

            var result = _authnResponseParser.Parse(xmlDocument, _options);

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

            var ex = Assert.Throws<ParsingException>(() => _authnResponseParser.Parse(xmlDocument, _options));
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

            var ex = Assert.Throws<ParsingException>(() => _authnResponseParser.Parse(xmlDocument, _options));
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

            var ex = Assert.Throws<ParsingException>(() => _authnResponseParser.Parse(xmlDocument, _options));
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

            var ex = Assert.Throws<ParsingException>(() => _authnResponseParser.Parse(xmlDocument, _options));
            Assert.Equal("Issue instant cannot be parsed", ex.Message);
        }

        private const string XmlWithEncryptedAssertion = "<samlp:Response xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\" ID=\"s22180325197d1c941b53cfbff893c17626898a429\" InResponseTo=\"_4035de012550465d8d29c2cd8fbb3a74\" Version=\"2.0\" IssueInstant=\"2018-04-19T17:17:20Z\" Destination=\"http://localhost:50000/saml-auth\"><saml:Issuer xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\">https://idp.ssocircle.com</saml:Issuer><samlp:Status xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\"><samlp:StatusCode xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\" Value=\"urn:oasis:names:tc:SAML:2.0:status:Success\"/></samlp:Status><saml:EncryptedAssertion xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\"><xenc:EncryptedData xmlns:xenc=\"http://www.w3.org/2001/04/xmlenc#\" xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\" Type=\"http://www.w3.org/2001/04/xmlenc#Element\"><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#aes128-cbc\"/><dsig:KeyInfo xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\"><xenc:EncryptedKey><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#rsa-1_5\"/><xenc:CipherData><xenc:CipherValue>DXVJckI5+BdgDRuUIloxuCFbLpo6mPppT26FMvzQGQVEqYCIAwHb9jo+wpifoDsnG1DsKoeS+DtoApxCE2snqgBHhlJSGHITaysjlKHB2chN5j01JT7tbhGEpCVy4Zlq9Cu3Lo1hmjAbmIhEUwr4Jkav8Y7JJPzpXp0rlwCQxuIC30IfxKrZlS6uE0lxB8Umo3d1Yjo/LJJnIRkfSBHbBKd1wQk8gAncd7U8+8+J/na5CgDPY78l/ndcfED5MgPb0uLA3F5Jl6s6dvhH4WbWUaGjIKv9NTN0H6znEhx2h5FAZqWOHuWk7EKiPo4W0Okvk7LTruMEAZpJu4PKGcLvIg==</xenc:CipherValue></xenc:CipherData></xenc:EncryptedKey></dsig:KeyInfo><xenc:CipherData><xenc:CipherValue>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</xenc:CipherValue></xenc:CipherData></xenc:EncryptedData></saml:EncryptedAssertion></samlp:Response>";
        private const string XmlWithEncryptedAttributes = "<samlp:Response xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\" ID=\"s22180325197d1c941b53cfbff893c17626898a429\" InResponseTo=\"_4035de012550465d8d29c2cd8fbb3a74\" Version=\"2.0\" IssueInstant=\"2018-04-19T17:17:20Z\" Destination=\"http://localhost:50000/saml-auth\"><saml:Issuer xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\">https://idp.ssocircle.com</saml:Issuer><samlp:Status xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\"><samlp:StatusCode xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\" Value=\"urn:oasis:names:tc:SAML:2.0:status:Success\"/></samlp:Status><saml:Assertion xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\" ID=\"s2ebd2bf18d7d82906a4590735ab8c4079a9da09c9\" IssueInstant=\"2018-04-19T17:17:20Z\" Version=\"2.0\"><saml:Issuer>https://idp.ssocircle.com</saml:Issuer><saml:Subject><saml:NameID Format=\"urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified\" NameQualifier=\"https://idp.ssocircle.com\">SamlOida</saml:NameID><saml:SubjectConfirmation Method=\"urn:oasis:names:tc:SAML:2.0:cm:bearer\"><saml:SubjectConfirmationData InResponseTo=\"_4035de012550465d8d29c2cd8fbb3a74\" NotOnOrAfter=\"2018-04-19T17:27:20Z\" Recipient=\"http://localhost:50000/saml-auth\"/></saml:SubjectConfirmation></saml:Subject><saml:Conditions NotBefore=\"2018-04-19T17:07:20Z\" NotOnOrAfter=\"2018-04-19T17:27:20Z\"><saml:AudienceRestriction><saml:Audience>SamlOida</saml:Audience></saml:AudienceRestriction></saml:Conditions><saml:AuthnStatement AuthnInstant=\"2018-04-19T17:17:07Z\" SessionIndex=\"s21208b061f202d1a77f81a06517704916f86d1401\"><saml:AuthnContext><saml:AuthnContextClassRef>urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport</saml:AuthnContextClassRef></saml:AuthnContext></saml:AuthnStatement><saml:AttributeStatement>\t\t\t<saml:EncryptedAttribute><xenc:EncryptedData xmlns:xenc=\"http://www.w3.org/2001/04/xmlenc#\" xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\" Type=\"http://www.w3.org/2001/04/xmlenc#Element\"><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#aes128-cbc\"/><dsig:KeyInfo xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\"><xenc:EncryptedKey><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#rsa-1_5\"/><xenc:CipherData><xenc:CipherValue>KOynqIlYR+WCc+hiKpbGTGn2wXT6qkPXru4qwCtnv2BxWay0rGncw2/xpcjZHu8922PAeThSwyChHM9DZZC+pATJYJ5j4MlFbVu7sMgIR65rLYDVAK7O7s2ur98MbIIj4ebeNO0Qgp4jQZb2m4NBrgJ9h/gWa5HNdznQdcunRvQAh9JVvQMHwgE2A4+kHgIEj7YSuhKM9WW/6I1tbiFLq0K2u8aYdxUSUWL2ZkkWfRPWcbpeda0SXtK6ddVbnnbr4Fw2rw3laHjsCYPJqFi6V+fCRhgqjGLDbgoCFDfSHsLJZJEjp8Aa8zODWuSNn96T8++M53fxtarCxwYFfXO/vw==</xenc:CipherValue></xenc:CipherData></xenc:EncryptedKey></dsig:KeyInfo><xenc:CipherData><xenc:CipherValue>n/a3Y/pDwRpHUrYsoj+OuKnNxH3/N35es8dgHs+evBNtP0qSOlNmmoDNX2h1i9IVaUZG/s35RVKhVioZGZ+vA0QRbvDdv9FYqtOd80Brc3aSOE7+WiqdrVDCZJXSv+4MYQ899Ve5RqdR+gj3RUzRHFBXjzzXusCcyPqlU+1zZh2jKs4HaUZaGv3cYL/13JYR1POQC2brk7qqaLpP3bjLkpPr9Ru8ek5fM3Xjh485tV+n70gR30P0BMLaQ+md6ivqyjzUZIBHj6bFB1+mIz8/qEMcVwj1Dur1ZqblrNXk+7D9f4D9+Fxpl6ucFJhnHBX43Du+DPoyOC7XlLuHWqs3zHeFfZ9vJC86mCsPoMFxN1w=</xenc:CipherValue></xenc:CipherData></xenc:EncryptedData></saml:EncryptedAttribute><saml:EncryptedAttribute><xenc:EncryptedData xmlns:xenc=\"http://www.w3.org/2001/04/xmlenc#\" xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\" Type=\"http://www.w3.org/2001/04/xmlenc#Element\"><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#aes128-cbc\"/><dsig:KeyInfo xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\"><xenc:EncryptedKey><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#rsa-1_5\"/><xenc:CipherData><xenc:CipherValue>XdyXPAtU8Zjd1z80pTjm3eIp5VU+4I2bBU0ztDReQ56dvi+hk2KvvO0+zNArwv42U1YaaS9asI+qAfMxqn4X23iTQq9vJVfrB87Dml70hF2fRwAvWkT5NjAL+HwKtwy1/2tU2brPZ3/hj5MhkbJaXD75BZwR2up5WZOrVb87wDmkECjnXvtPEQJrHuKEAe/KaTrJzuwyQX4t7ewNy6YoiSye2txetwEeZD6jLI15Ie1T+T40RvXxFXQvJ2awtOofePSOcn6uVEh0GR5uTWC2P6Fw5kKEJZ8Gi519ceVBpkASQaMqXQgJg+l8HUsk3OKOu5blda4aU2mwizWBrClHOQ==</xenc:CipherValue></xenc:CipherData></xenc:EncryptedKey></dsig:KeyInfo><xenc:CipherData><xenc:CipherValue>quUb8EgUVir9UvQN/EWtUYlqptRLb8isAgerT2DKn32JMWykqnJAO6bW27znHnMQoQPT3lUAsAq46/Rnbmhcm7GPPIXaLQOw/c7J8iNPwjJWPFA3MrIPtzRvTBMAk3IlXNTfCbRtFqfBYad3w36wfyFC3IZh+KlZfDnFMPADiYTafdPgtr9F8pwDJiP1yog2dhFZtunB70fOP2VBIt6PPJixJGe7iu/EHvNw303MEYTtukxxcejBqWfKPpLyIsAa9A7MWOeWAAbWzHARv6wBeqkPKKVNOgR+z/DbGSf3YPkwaC8KIYnxKd4I3Veic8fdPECW/ONzNUH2LQRFZqzmig==</xenc:CipherValue></xenc:CipherData></xenc:EncryptedData></saml:EncryptedAttribute><saml:EncryptedAttribute><xenc:EncryptedData xmlns:xenc=\"http://www.w3.org/2001/04/xmlenc#\" xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\" Type=\"http://www.w3.org/2001/04/xmlenc#Element\"><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#aes128-cbc\"/><dsig:KeyInfo xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\"><xenc:EncryptedKey><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#rsa-1_5\"/><xenc:CipherData><xenc:CipherValue>aCtuo3ij2MWLqM6VY+WTrpfHJV9DM4kfemus5pD65JsB+Aj8YCVSmBX71gixteUNwG1awV5RMvcm/5cr+tYj4tq6AZbJUG+fOkcnUaowvgZ/v9TeRFDsrpb/vmr35KyqWRpQMWAPl2KBLfEaKDi1F38gSbIvDE1YXXg+/uuPPGqzA739o1BVkz13Hxnb/oUKK+15b6DW4QLAB5qrqh3GgfJruPmXasUeLurfAZrnSnzMTsIHiY/BDWYU4Nb69iYWJVUKYx67vWCkmLBkJM5C/t8yWb2aAyozzX49W4st5e8ESxY01StAA2Vn1vIXJf23mDomDVKnVy3cYRSXKFUqiA==</xenc:CipherValue></xenc:CipherData></xenc:EncryptedKey></dsig:KeyInfo><xenc:CipherData><xenc:CipherValue>bGbqGsbyYWL3HqDd1k0mqtQfnTQ9KBj4LPEuaOhKTVQWPtiS25UaO59/8noKF6XEbrIU1rwQO7HgRVNdw37kt9ZfLIMPEpaN27MdFdGoky4HR0eCxwjGCiiwGMF9Y0F2zmXtIivJmtJ2sU/VcaeknUyVa+JghQ4RWfd/xBQxXwttzzVcx+qqrqhbJAxfLPZGmsF//t3ZU3gaD5zpwx9dAhMUCP+nk1jRcfxWXoJAf+XNpLYqiVxJbm8EGJTOu6oIlUh10OSvb96aV04mn98heJQ3Y8WUaEU16OQx1a5oA5YDu3nZQCHVDVfoX9JrOVCLU6spj4bsTTMOmPVegGmEbQ==</xenc:CipherValue></xenc:CipherData></xenc:EncryptedData></saml:EncryptedAttribute><saml:EncryptedAttribute><xenc:EncryptedData xmlns:xenc=\"http://www.w3.org/2001/04/xmlenc#\" xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\" Type=\"http://www.w3.org/2001/04/xmlenc#Element\"><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#aes128-cbc\"/><dsig:KeyInfo xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\"><xenc:EncryptedKey><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#rsa-1_5\"/><xenc:CipherData><xenc:CipherValue>2Fh3isYhULpgXpNqRktXNhBchcY7EOgKZJ3wsTzNsUtgQE5TIsUEw9uOSjMiUJ9knl63NLpzjjXoaDRWkf+LYCwEaBkusfFt5R0GXFNzpqIWJmxBYVs7PU1gtGpq4dW+MhxHhKWWTwlsU4Gh5BkTdDIXrXERRSPzgrV71QjMzYGMdjDjk+oupKGzJcQztgAxV9kt3XtBZE/s7/MYC9K5vPQbzk7AsX/xuR9N8aB2AYpjLtmE4QWT8eLjnAyM89FWT4Oq02+NK6BOmWSOmjyWjkvDOQTMxgB1Cl3d2gafsxmG/oz2u+yP8JzDml5zPidlv/C4dMpoWrbd9hQ7WKtuBg==</xenc:CipherValue></xenc:CipherData></xenc:EncryptedKey></dsig:KeyInfo><xenc:CipherData><xenc:CipherValue>gbD71QYVuhTMpbyhpmmrmVwLb01CM5Bus6eGbvTTxHWnU6KubCV1tLxGG1rmPxmb4v4tx5j9UmzltHcKKBW/9ze032U91zUfyfwpAnv0AosEnxQr7DNWoS6JsEgWFBG9jTUl7rOlJ7sgNNNRQY5DxTTTy91CQLhXgypIOe9aeTn+WtEJXaZgrNZrRD6uDcGEbfD5TFmWg88Z+Rf4jpU0G0ugjZwg5G/xBuS0UyX60EhrTL50jBFptYYuFSYZekrcTETh9LPvsMdxmVmPtfalWgGv3Z9rHqkyqGdpkhqqqoQNQBXTZte7CINPbzz/c+FX46BFJCubLrKv7wf7cQKw6w==</xenc:CipherValue></xenc:CipherData></xenc:EncryptedData></saml:EncryptedAttribute></saml:AttributeStatement></saml:Assertion></samlp:Response>";
        
        [Fact]
        public void ShouldDecryptAssertions()
        {
            var doc = new XmlDocument();
            doc.LoadXml(XmlWithEncryptedAssertion);

            _options.ServiceProviderCertificate = new X509Certificate2(File.ReadAllBytes("PrivateTestCert.pfx"), "test");

            var result = _authnResponseParser.Parse(doc, _options);

            Assert.NotNull(result.Assertions.SingleOrDefault());
        }

        [Fact]
        public void ShouldDecryptAttributes()
        {
            var doc = new XmlDocument();
            doc.LoadXml(XmlWithEncryptedAttributes);

            _options.ServiceProviderCertificate = new X509Certificate2(File.ReadAllBytes("PrivateTestCert.pfx"), "test");

            var result = _authnResponseParser.Parse(doc, _options);

            Assert.Equal(4, result.Assertions.Single().Attributes.Count());
        }
    }
}
