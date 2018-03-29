using SamlOida.Model;
using System;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public class AuthnResponseParser : ISamlMessageParser<SamlAuthnResponseMessage>
    {
        public SamlAuthnResponseMessage Parse(XmlDocument message)
        {
            var ns = SamlXmlExtensions.NamespaceManager;

            var responseNode = message.SelectSingleNode($"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:Response", ns);
            if (responseNode == null)
                throw new ParsingException("Element Response missing");

            var statusCodeNode =
                responseNode.SelectSingleNode(
                    $"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:Status/{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:StatusCode/@Value", ns);
            if (string.IsNullOrEmpty(statusCodeNode?.InnerText))
                throw new ParsingException("Element 'StatusCode' missing");

            var result = new SamlAuthnResponseMessage();

            //TODO: Extract oasis-namespace
            if (statusCodeNode.InnerText.Equals("urn:oasis:names:tc:SAML:2.0:status:Success", StringComparison.OrdinalIgnoreCase))
                result.Success = true;

            //Response can contain signature itself!
            var responseSignature = responseNode.SelectSingleNode("ds:Signature", ns);
            if (responseSignature != null)
            {
                result.IsSigned = true;
                //TODO: Check signature, throw if invalid
            }

            var issueInstantNode = responseNode.SelectSingleNode("@IssueInstant", ns);
            if (string.IsNullOrEmpty(issueInstantNode?.InnerText))
                throw new ParsingException("Attribute 'IssueInstant' missing");
            
            if (!DateTime.TryParse(issueInstantNode.InnerText, out var issueInstant))
                throw new ParsingException("Issue instant cannot be parsed");

            result.IssueInstant = issueInstant;

            var assertions = SamlXmlExtensions.ParseAssertions(responseNode);
            result.Assertions = assertions;
            
            return result;
        }
    }
    
}
