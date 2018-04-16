using SamlOida.Model;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public abstract class SamlMessageParser<TMessageContext>
        where TMessageContext : SamlMessage, new()
    {
        protected abstract string RootElementName { get; }

        internal TMessageContext Parse(XmlDocument message, X509Certificate2 idpCertificate = null)
        {
            var msg = new TMessageContext();

            var encryptedXml = new EncryptedXml(message);
            encryptedXml.DecryptDocument();

            var rootNode = message.SelectSingleNode($"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:{RootElementName}", SamlXmlExtensions.NamespaceManager);
            if (rootNode == null)
                throw new ParsingException($"Element '{RootElementName}' missing.");

            SamlXmlExtensions.ParseStandardElements((XmlElement)rootNode, msg);

            var signatureNode = message.DocumentElement.SelectSingleNode("ds:Signature", SamlXmlExtensions.NamespaceManager);

            if (signatureNode != null && idpCertificate != null)
            {
                var signedXml = new SignedXml();
                signedXml.LoadXml((XmlElement)signatureNode);
                msg.HasValidSignature = signedXml.CheckSignature(idpCertificate, false);
            }

            return ParseInternal(rootNode, msg, idpCertificate);

        }
        protected abstract TMessageContext ParseInternal(XmlNode logoutResponseNode, TMessageContext messageObject, X509Certificate2 idpCertificate);
    }
}