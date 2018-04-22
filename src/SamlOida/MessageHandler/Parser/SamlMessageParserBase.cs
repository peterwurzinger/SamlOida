using SamlOida.Model;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public abstract class SamlMessageParserBase<TMessageContext> : ISamlMessageParser<TMessageContext>
        where TMessageContext : SamlMessage, new()
    {
        protected abstract string RootElementName { get; }

        public TMessageContext Parse(XmlDocument message, SamlOptions options)
        {
            var msg = new TMessageContext();

            var encryptedXml = new X509EncryptedXmlAdapter(message, options.ServiceProviderCertificate);
            encryptedXml.DecryptDocument();

            var rootNode = message.SelectSingleNode($"{SamlAuthenticationDefaults.SamlProtocolNsPrefix}:{RootElementName}", SamlXmlExtensions.NamespaceManager);
            if (rootNode == null)
                throw new ParsingException($"Element '{RootElementName}' missing.");

            SamlXmlExtensions.ParseStandardElements((XmlElement)rootNode, msg);

            var signatureNode = message.DocumentElement.SelectSingleNode("ds:Signature", SamlXmlExtensions.NamespaceManager);

            if (signatureNode != null && options.IdentityProviderCertificate != null)
            {
                var signedXml = new SignedXml(message);
                signedXml.LoadXml((XmlElement)signatureNode);
                msg.HasValidSignature = signedXml.CheckSignature(options.IdentityProviderCertificate, false);
            }

            return ParseInternal(rootNode, msg, options);

        }
        protected abstract TMessageContext ParseInternal(XmlNode logoutResponseNode, TMessageContext messageObject, SamlOptions options);
    }
}