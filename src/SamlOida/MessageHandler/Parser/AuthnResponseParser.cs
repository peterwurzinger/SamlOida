using SamlOida.Model;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public class AuthnResponseParser : SamlMessageParserBase<SamlAuthnResponseMessage>
    {
        protected override string RootElementName => "Response";

        protected override SamlAuthnResponseMessage ParseInternal(XmlNode responseNode, SamlAuthnResponseMessage result, SamlOptions options)
        {
            var statusCodeNode =
                responseNode.SelectSingleNode($"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:Status/{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:StatusCode/@Value", SamlXmlExtensions.NamespaceManager);
            if (statusCodeNode == null || string.IsNullOrEmpty(statusCodeNode.InnerText))
                throw new ParsingException("Element 'StatusCode' missing");

            //TODO: Extract oasis-namespace
            if (statusCodeNode.InnerText.Equals("urn:oasis:names:tc:SAML:2.0:status:Success", StringComparison.OrdinalIgnoreCase))
                result.Success = true;

            var assertions = ParseAssertions(responseNode, options);
            result.Assertions = assertions;

            return result;
        }

        private static IEnumerable<SamlAssertion> ParseAssertions(XmlNode responseNode, SamlOptions options)
        {
            //All Assertions as direct child of Response-Element, or wrapped in EncryptedAssertion
            var assertions = responseNode.SelectNodes($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:Assertion | {SamlAuthenticationDefaults.SamlAssertionNsPrefix}:EncryptedAssertion/{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:Assertion", SamlXmlExtensions.NamespaceManager);

            var result = new List<SamlAssertion>();
            for (var i = 0; i < assertions.Count; i++)
            {
                var assertion = new SamlAssertion();
                var assertionNode = assertions[i];

                assertion.Issuer = assertionNode.SelectSingleNode($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:Issuer", SamlXmlExtensions.NamespaceManager)?.InnerText;
                assertion.Id = assertionNode.SelectSingleNode("@ID", SamlXmlExtensions.NamespaceManager)?.Value;

                var signatureNode = assertionNode.SelectSingleNode("ds:Signature", SamlXmlExtensions.NamespaceManager);

                if (signatureNode != null && options.IdentityProviderCertificate != null)
                {
                    var signedXml = new SignedXml(assertionNode.OwnerDocument);
                    signedXml.LoadXml((XmlElement)signatureNode);
                    assertion.HasValidSignature = signedXml.CheckSignature(options.IdentityProviderCertificate, true);
                }

                var attributeStatements = assertionNode.SelectNodes($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:AttributeStatement", SamlXmlExtensions.NamespaceManager);
                assertion.Attributes = ParseAttributeStatements(attributeStatements, options);

                assertion.SessionIndex = assertionNode.SelectSingleNode($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:AuthnStatement/@SessionIndex", SamlXmlExtensions.NamespaceManager)?.Value;
                assertion.SubjectNameId = assertionNode.SelectSingleNode($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:Subject/{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:NameID", SamlXmlExtensions.NamespaceManager)?.InnerText;

                result.Add(assertion);
            }

            return result;
        }

        private static IEnumerable<SamlAttribute> ParseAttributeStatements(XmlNodeList attributeStatements, SamlOptions options)
        {
            var parsedAttributes = new List<SamlAttribute>();
            for (var i = 0; i < attributeStatements.Count; i++)
            {
                var attributeStatementNode = attributeStatements[i];
                var attributeNodes = attributeStatementNode.SelectNodes($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:Attribute", SamlXmlExtensions.NamespaceManager);

                parsedAttributes.AddRange(ParseAttributes(attributeNodes));

                var encryptedAttributeNodes = attributeStatementNode.SelectNodes($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:EncryptedAttribute/{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:Attribute", SamlXmlExtensions.NamespaceManager);
                if (encryptedAttributeNodes.Count > 0)
                {
                    //2. Parse as usual attributes
                    parsedAttributes.AddRange(ParseAttributes(encryptedAttributeNodes));
                }
            }

            return parsedAttributes;
        }

        private static IEnumerable<SamlAttribute> ParseAttributes(XmlNodeList attributes)
        {
            var result = new List<SamlAttribute>();
            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                var attributeName = attribute.SelectSingleNode("@Name", SamlXmlExtensions.NamespaceManager);
                if (attributeName == null || string.IsNullOrEmpty(attributeName.InnerText))
                    throw new ParsingException("Attribute name is missing");

                var attributeObject = new SamlAttribute
                {
                    Name = attributeName.InnerText,
                    IsEncrypted = attribute.Name == "EncryptedAttribute"
                };

                var attributeNameFormat = attribute.SelectSingleNode("@NameFormat", SamlXmlExtensions.NamespaceManager);
                if (!string.IsNullOrEmpty(attributeNameFormat?.InnerText))
                    attributeObject.NameFormat = attributeNameFormat.InnerText;

                var attributeFriendlyName = attribute.SelectSingleNode("@FriendlyName", SamlXmlExtensions.NamespaceManager);
                if (!string.IsNullOrEmpty(attributeFriendlyName?.InnerText))
                    attributeObject.FriendlyName = attributeFriendlyName.InnerText;

                var attributeValues = attribute.SelectNodes($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:AttributeValue", SamlXmlExtensions.NamespaceManager);
                for (var j = 0; j < attributeValues.Count; j++)
                {
                    var attributeValue = attributeValues[j];
                    if (!string.IsNullOrEmpty(attributeValue?.InnerText))
                        attributeObject.Values.Add(attributeValue.InnerText);
                }

                result.Add(attributeObject);
            }
            return result;
        }
    }

}
