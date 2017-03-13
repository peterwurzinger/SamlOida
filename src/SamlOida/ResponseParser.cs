using System;
using System.Linq;
using System.Xml;

namespace SamlOida
{
    public class ResponseParser
    {
        private readonly XmlDocument _document;
        private readonly ResponseParsingResult _result;

        public ResponseParser(XmlDocument document)
        {
            _document = document;
            _result = new ResponseParsingResult();
        }

        //https://docs.oasis-open.org/security/saml/v2.0/saml-schema-protocol-2.0.xsd
        public ResponseParsingResult Parse()
        {
            var ns = new XmlNamespaceManager(_document.NameTable);
            ns.AddNamespace(SamlDefaults.SamlProtocolNsPrefix, SamlDefaults.SamlProtocolNamespace);
            ns.AddNamespace(SamlDefaults.SamlAssertionNsPrefix, SamlDefaults.SamlAssertionNamespace);

            var responseNode = _document.SelectSingleNode($"{SamlDefaults.SamlProtocolNsPrefix}:Response");
            if (responseNode == null)
                //TODO: Find better exception type
                throw new Exception("Element Response missing");

            var statusCodeNode = responseNode.SelectSingleNode($"{SamlDefaults.SamlProtocolNsPrefix}:Status/{SamlDefaults.SamlProtocolNsPrefix}:StatusCode/@Value", ns);
            if (string.IsNullOrEmpty(statusCodeNode?.InnerText))
                throw new Exception("Element StatusCode missing");

            //TODO: Extract oasis-namespace
            if (!statusCodeNode.InnerText.Equals("urn:oasis:names:tc:SAML:2.0:status:Success", StringComparison.OrdinalIgnoreCase))
                return new ResponseParsingResult {Succeeded = false};

            var issueInstantNode = responseNode.SelectSingleNode("@IssueInstant");
            if (string.IsNullOrEmpty(issueInstantNode?.InnerText))
                throw new Exception("Attribute IssueInstant missing");

            //TODO: Verify timestamp

            //TODO: Zero or more elements of either Assertion or EncryptedAssertion can occour. Extract!
            var assertionNodes = responseNode.SelectNodes($"{SamlDefaults.SamlAssertionNsPrefix}:Assertion", ns);
            if (assertionNodes.Count > 0)
            {
                ParseAssertion(assertionNodes);
            }
            else
            {
                responseNode.SelectNodes($"{SamlDefaults.SamlAssertionNsPrefix}:EncryptedAssertion", ns);
                //1. Decrypt
                //2. Parse as usualAssertions
                throw new NotImplementedException();
            }

            return _result;
        }

        private void ParseAssertion(XmlNodeList assertions)
        {
            foreach (var assertionNode in assertions.Cast<XmlNode>())
            {
                var signatureNode = assertionNode.SelectSingleNode($"{SamlDefaults.SamlAssertionNsPrefix}:Signature");
                if (signatureNode != null)
                {
                    //TODO: Check signature
                    throw new NotImplementedException();
                }
            }

        }

    }
}
