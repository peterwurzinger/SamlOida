using System.Security.Cryptography.X509Certificates;
using SamlOida.Model;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public class LogoutResponseParser : SamlMessageParser<SamlLogoutResponseMessage>
    {
        protected override string RootElementName => "LogoutResponse";

        protected override SamlLogoutResponseMessage ParseInternal(XmlNode logoutResponseNode, SamlLogoutResponseMessage result, X509Certificate2 idpCertificate)
        { 
            var statusCodeNode = logoutResponseNode.SelectSingleNode($"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:Status/{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:StatusCode/@Value", SamlXmlExtensions.NamespaceManager);
            if (statusCodeNode == null)
                throw new ParsingException("Node 'StatusCode' missing.");

            result.Success = statusCodeNode.Value == "urn:oasis:names:tc:SAML:2.0:status:Success";

            return result;
        }
    }
}
