using SamlOida.Model;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public class LogoutRequestParser : SamlMessageParserBase<SamlLogoutRequestMessage>
    {
        protected override string RootElementName => "LogoutRequest";

        protected override SamlLogoutRequestMessage ParseInternal(XmlNode logoutRequestNode, SamlLogoutRequestMessage result, SamlOptions options)
        {
            var nameIdNode = logoutRequestNode.SelectSingleNode($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:NameID", SamlXmlExtensions.NamespaceManager);
            if (nameIdNode == null)
                throw new ParsingException("Node 'NameID' missing.");

            result.NameId = nameIdNode.InnerText;
            result.SessionIndex = logoutRequestNode.SelectSingleNode($"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:SessionIndex", SamlXmlExtensions.NamespaceManager)?.InnerText;

            return result;
        }
    }
}
