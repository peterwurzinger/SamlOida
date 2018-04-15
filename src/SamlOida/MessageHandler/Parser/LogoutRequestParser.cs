using SamlOida.Model;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public class LogoutRequestParser : ISamlMessageParser<SamlLogoutRequestMessage>
    {
        public SamlLogoutRequestMessage Parse(XmlDocument message)
        {
            var logoutRequestNode = message.SelectSingleNode($"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:LogoutRequest", SamlXmlExtensions.NamespaceManager);
            if (logoutRequestNode == null)
                throw new ParsingException("Element 'LogoutRequest' missing.");

            var result = new SamlLogoutRequestMessage();

            SamlXmlExtensions.ParseStandardElements(message, result);

            var nameIdNode = message.SelectSingleNode($"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:LogoutRequest/{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:NameID", SamlXmlExtensions.NamespaceManager);
            if (nameIdNode == null)
                throw new ParsingException("Node 'NameID' missing.");

            result.NameId = nameIdNode.InnerText;

            return result;
        }
    }
}
