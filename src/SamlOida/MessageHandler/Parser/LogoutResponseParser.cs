using SamlOida.Model;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public class LogoutResponseParser : ISamlMessageParser<SamlLogoutResponseMessage>
    {
        public SamlLogoutResponseMessage Parse(XmlDocument message)
        {
            var logoutResponseNode = message.SelectSingleNode($"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:LogoutResponse", SamlXmlExtensions.NamespaceManager);
            if (logoutResponseNode == null)
                throw new ParsingException("Element 'LogoutResponse' missing.");

            var result = new SamlLogoutResponseMessage();

            SamlXmlExtensions.ParseStandardElements(message, result);

            var statusCodeNode = message.SelectSingleNode($"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:LogoutResponse/{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:Status/{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:StatusCode", SamlXmlExtensions.NamespaceManager);
            if (statusCodeNode == null)
                throw new ParsingException("Node 'StatusCode' missing.");

            result.Success = statusCodeNode.Value == "urn:oasis:names:tc:SAML:2.0:status:Success";

            return result;
        }
    }
}
