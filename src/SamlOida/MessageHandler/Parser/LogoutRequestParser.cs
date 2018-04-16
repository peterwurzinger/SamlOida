using SamlOida.Model;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public class LogoutRequestParser : SamlMessageParser<SamlLogoutRequestMessage>
    {
        protected override string RootElementName => "LogoutRequest";

        protected override SamlLogoutRequestMessage ParseInternal(XmlNode logoutRequestNode, SamlLogoutRequestMessage result, X509Certificate2 idpCertificate)
        {
            var nameIdNode = logoutRequestNode.SelectSingleNode($"{SamlAuthenticationDefaults.SamlAssertionNsPrefix}:NameID", SamlXmlExtensions.NamespaceManager);
            if (nameIdNode == null)
                throw new ParsingException("Node 'NameID' missing.");

            result.NameId = nameIdNode.InnerText;

            return result;
        }
    }
}
