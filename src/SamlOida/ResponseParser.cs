using System;
using System.Xml;

namespace SamlOida
{
    internal class ResponseParser
    {
        private readonly XmlDocument _document;
        private readonly ResponseParsingResult _result;
        private readonly XmlNamespaceManager _ns;

        internal ResponseParser(XmlDocument document)
        {
            _document = document;
            _document.PreserveWhitespace = true;

            _result = new ResponseParsingResult();
            _ns = new XmlNamespaceManager(_document.NameTable);
            _ns.AddNamespace(SamlDefaults.SamlProtocolNsPrefix, SamlDefaults.SamlProtocolNamespace);
            _ns.AddNamespace(SamlDefaults.SamlAssertionNsPrefix, SamlDefaults.SamlAssertionNamespace);
            _ns.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
        }

        //https://docs.oasis-open.org/security/saml/v2.0/saml-schema-protocol-2.0.xsd
        internal ResponseParsingResult Parse()
        {
            var responseNode = _document.SelectSingleNode($"{SamlDefaults.SamlProtocolNsPrefix}:Response", _ns);
            if (responseNode == null)
                //TODO: Find better exception type
                throw new ParsingException("Element Response missing");

            var statusCodeNode =
                responseNode.SelectSingleNode(
                    $"{SamlDefaults.SamlProtocolNsPrefix}:Status/{SamlDefaults.SamlProtocolNsPrefix}:StatusCode/@Value", _ns);
            if (string.IsNullOrEmpty(statusCodeNode?.InnerText))
                throw new ParsingException("Element StatusCode missing");

            //TODO: Extract oasis-namespace
            if (
                !statusCodeNode.InnerText.Equals("urn:oasis:names:tc:SAML:2.0:status:Success", StringComparison.OrdinalIgnoreCase))
                return new ResponseParsingResult
                {
                    Succeeded = false
                };

            //Response can contain signature itself!
            var responseSignature = responseNode.SelectSingleNode($"ds:Signature", _ns);
            if (responseSignature != null)
            {
                //TODO: Check signature
            }

            var issueInstantNode = responseNode.SelectSingleNode("@IssueInstant", _ns);
            if (string.IsNullOrEmpty(issueInstantNode?.InnerText))
                throw new ParsingException("Attribute IssueInstant missing");

            //TODO: Verify timestamp

            //TODO: Zero or more elements of either Assertion or EncryptedAssertion can occour. Extract!
            var assertionNodes = responseNode.SelectNodes($"{SamlDefaults.SamlAssertionNsPrefix}:Assertion", _ns);
            ParseAssertion(assertionNodes);
            var encryptedAssertionNodes = responseNode.SelectNodes($"{SamlDefaults.SamlAssertionNsPrefix}:EncryptedAssertion", _ns);

            if (encryptedAssertionNodes.Count > 0) { 
                //1. Decrypt
                //2. Parse as usual assertions
                throw new NotImplementedException("Parsing encrypted assertions is currently not supported");
            }
            return _result;
        }

        private void ParseAssertion(XmlNodeList assertions)
        {
            for (var i = 0; i < assertions.Count; i++)
            {
                var assertionNode = assertions[i];
                var signatureNode = assertionNode.SelectSingleNode("ds:Signature", _ns);
                if (signatureNode != null)
                {
                    //TODO: Check signature
                    //throw new NotImplementedException("Validating assertion signatures is currently not supported");
                }
                var attributeStatements = assertionNode.SelectNodes($"{SamlDefaults.SamlAssertionNsPrefix}:AttributeStatement", _ns);
                ParseAttributeStatements(attributeStatements);
            }
        }

        private void ParseAttributeStatements(XmlNodeList attributeStatements)
        {
            for (var i = 0; i < attributeStatements.Count; i++)
            {
                var attributeStatementNode = attributeStatements[i];
                var attributeNodes = attributeStatementNode.SelectNodes($"{SamlDefaults.SamlAssertionNsPrefix}:Attribute", _ns);
                ParseAttributes(attributeNodes);

                var encryptedAttributeNodes = attributeStatementNode.SelectNodes($"{SamlDefaults.SamlAssertionNsPrefix}:EncryptedAttribute", _ns);
                if (encryptedAttributeNodes.Count > 0)
                {
                    //1. Decrypt
                    //2. Parse as usual attributes
                    throw new NotImplementedException("Parsing encrypted attributes is currently not supported");
                }
            }
        }

        private void ParseAttributes(XmlNodeList attributes)
        {
            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                var attributeName = attribute.SelectSingleNode("@Name", _ns);
                if (string.IsNullOrEmpty(attributeName?.InnerText))
                    throw new ParsingException("Attribute name is missing");

                var attributeObject = new SamlAttribute
                {
                    Name = attributeName.InnerText
                };

                var attributeNameFormat = attribute.SelectSingleNode("@NameFormat", _ns);
                if (!string.IsNullOrEmpty(attributeNameFormat?.InnerText))
                    attributeObject.NameFormat = attributeNameFormat.InnerText;

                var attributeFriendlyName = attribute.SelectSingleNode("@FriendlyName", _ns);
                if (!string.IsNullOrEmpty(attributeFriendlyName?.InnerText))
                    attributeObject.FriendlyName = attributeFriendlyName.InnerText;

                var attributeValues = attribute.SelectNodes($"{SamlDefaults.SamlAssertionNsPrefix}:AttributeValue", _ns);
                for (var j = 0; j < attributeValues.Count; j++)
                {
                    var attributeValue = attributeValues[j];
                    if (!string.IsNullOrEmpty(attributeValue?.InnerText))
                        attributeObject.Values.Add(attributeValue.InnerText);
                }

                _result.Attributes.Add(attributeObject);
            }
        }
    }
}
