using SamlOida.Model;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public class LogoutRequestParser : SamlMessageParser<SamlLogoutRequestMessage>
    {
        protected override string RootElementName => "LogoutRequest";

        protected override SamlLogoutRequestMessage ParseInternal(XmlNode logoutRequestNode, SamlLogoutRequestMessage result, SamlOptions options)
        {
            var nameIdNode = logoutRequestNode.SelectSingleNode($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:NameID", SamlXmlExtensions.NamespaceManager);
            if (nameIdNode == null)
                throw new ParsingException("Node 'NameID' missing.");

            result.NameId = nameIdNode.InnerText;

            return result;
        }
    }
}
