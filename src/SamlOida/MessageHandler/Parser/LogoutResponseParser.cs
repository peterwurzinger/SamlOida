using SamlOida.Model;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public class LogoutResponseParser : SamlMessageParserBase<SamlLogoutResponseMessage>
    {
        protected override string RootElementName => "LogoutResponse";

        protected override SamlLogoutResponseMessage ParseInternal(XmlNode logoutResponseNode, SamlLogoutResponseMessage result, SamlOptions options)
        {
            var statusCodeNode = logoutResponseNode.SelectSingleNode($"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:Status/{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:StatusCode/@Value", SamlXmlExtensions.NamespaceManager);
            if (statusCodeNode == null)
                throw new ParsingException("Node 'StatusCode' missing.");

            result.Success = statusCodeNode.Value == "urn:oasis:names:tc:SAML:2.0:status:Success";

            result.InResponseTo = logoutResponseNode.Attributes["InResponseTo"]?.Value;

            return result;
        }
    }
}
