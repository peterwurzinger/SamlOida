using SamlOida.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public static class SamlXmlExtensions
    {

        private static XmlNamespaceManager _namespaceManager;
        public static XmlNamespaceManager NamespaceManager
        {
            get
            {
                if (_namespaceManager != null)
                    return _namespaceManager;

                //TODO: Lock?
                _namespaceManager = new XmlNamespaceManager(new NameTable());
                _namespaceManager.AddNamespace(SamlAuthenticationDefaults.SamlProtocolNsPrefix, SamlAuthenticationDefaults.SamlProtocolNamespace);
                _namespaceManager.AddNamespace(SamlAuthenticationDefaults.SamlAssertionNsPrefix, SamlAuthenticationDefaults.SamlAssertionNamespace);
                _namespaceManager.AddNamespace(SamlAuthenticationDefaults.XmlSignatureNsPrefix, SignedXml.XmlDsigNamespaceUrl);

                return _namespaceManager;
            }
        }

        internal static void PropagateStandardElements(XmlDocument doc, XmlElement element, SamlMessage message)
        {
            element.SetAttribute($"xmlns:{SamlAuthenticationDefaults.SamlAssertionNsPrefix}", SamlAuthenticationDefaults.SamlAssertionNamespace);
            element.SetAttribute($"xmlns:{SamlAuthenticationDefaults.SamlProtocolNsPrefix}", SamlAuthenticationDefaults.SamlProtocolNamespace);

            //TODO: Use something else than Guid.NewGuid ?
            element.SetAttribute("ID", $"Issuer_{Guid.NewGuid()}");
            element.SetAttribute("Version", "2.0");

            //Does the Standardportal differ from SAML-Standard?
            element.SetAttribute("IssueInstant", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));

            element.SetAttribute("Destination", message.Destination);

            var issuerElement = doc.CreateElement(SamlAuthenticationDefaults.SamlAssertionNsPrefix, "Issuer", SamlAuthenticationDefaults.SamlAssertionNamespace);
            issuerElement.InnerText = message.Issuer;

            element.AppendChild(issuerElement);

        }

        internal static void ParseStandardElements(XmlDocument doc, SamlMessage message)
        {
            message.Id = doc.DocumentElement.GetAttribute("ID");

            //Does the Standardportal differ from SAML-Standard?
            message.IssueInstant = DateTime.Parse(doc.DocumentElement.GetAttribute("IssueInstant"));

            message.Destination = doc.DocumentElement.GetAttribute("Destination");

            message.Issuer = doc.DocumentElement.GetElementsByTagName("Issuer", SamlAuthenticationDefaults.SamlAssertionNamespace)[0].Value;
        }

        internal static IEnumerable<SamlAssertion> ParseAssertions(XmlNode responseNode)
        {
            var assertions = responseNode.SelectNodes($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:Assertion", NamespaceManager);
            var result = new List<SamlAssertion>();
            for (var i = 0; i < assertions.Count; i++)
            {
                var assertion = new SamlAssertion();
                var assertionNode = assertions[i];
                var signatureNode = assertionNode.SelectSingleNode("ds:Signature", NamespaceManager);

                if (signatureNode != null)
                {
                    assertion.IsSigned = true;
                    //TODO: Check signature, throw if invalid
                }

                var attributeStatements = assertionNode.SelectNodes($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:AttributeStatement", NamespaceManager);
                assertion.Attributes = ParseAttributeStatements(attributeStatements);

                result.Add(assertion);
            }

            return result;
        }

        private static IEnumerable<SamlAttribute> ParseAttributeStatements(XmlNodeList attributeStatements)
        {
            var parsedAttributes = new List<SamlAttribute>();
            for (var i = 0; i < attributeStatements.Count; i++)
            {
                var attributeStatementNode = attributeStatements[i];
                var attributeNodes = attributeStatementNode.SelectNodes($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:Attribute", NamespaceManager);

                parsedAttributes.AddRange(ParseAttributes(attributeNodes));

                var encryptedAttributeNodes = attributeStatementNode.SelectNodes($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:EncryptedAttribute", NamespaceManager);
                if (encryptedAttributeNodes.Count > 0)
                {
                    parsedAttributes.AddRange(ParseAttributes(encryptedAttributeNodes));
                    //1. Decrypt
                    //2. Parse as usual attributes
                    throw new NotImplementedException("Parsing encrypted attributes is currently not supported");
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
                var attributeName = attribute.SelectSingleNode("@Name", NamespaceManager);
                if (string.IsNullOrEmpty(attributeName?.InnerText))
                    throw new ParsingException("Attribute name is missing");

                var attributeObject = new SamlAttribute
                {
                    Name = attributeName.InnerText,
                    IsEncrypted = attribute.Name == "EncryptedAttribute"
                };

                var attributeNameFormat = attribute.SelectSingleNode("@NameFormat", NamespaceManager);
                if (!string.IsNullOrEmpty(attributeNameFormat?.InnerText))
                    attributeObject.NameFormat = attributeNameFormat.InnerText;

                var attributeFriendlyName = attribute.SelectSingleNode("@FriendlyName", NamespaceManager);
                if (!string.IsNullOrEmpty(attributeFriendlyName?.InnerText))
                    attributeObject.FriendlyName = attributeFriendlyName.InnerText;

                var attributeValues = attribute.SelectNodes($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:AttributeValue", NamespaceManager);
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
