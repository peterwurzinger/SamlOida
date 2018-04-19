using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    /// <inheritdoc />
    /// <summary>
    /// Wrapper for EncryptedXml when used in SAML-Context for EncryptedAssertion and EncryptedAttribute
    /// </summary>
    internal class X509EncryptedXmlAdapter : EncryptedXml
    {
        private readonly X509Certificate2 _certificate;

        internal X509EncryptedXmlAdapter(XmlDocument doc, X509Certificate2 certificate) : base(doc)
        {
            if (certificate == null)
                return;

            if (!certificate.HasPrivateKey)
                throw new ArgumentException($"Certificate {certificate.FriendlyName} contains no private key.", nameof(certificate));

            _certificate = certificate;

        }

        public override byte[] DecryptEncryptedKey(EncryptedKey encryptedKey)
        {
            //If there is no certificate we're done.
            if (_certificate == null)
                return base.DecryptEncryptedKey(encryptedKey);

            if (encryptedKey == null)
                throw new ArgumentNullException(nameof(encryptedKey));
            
            return DecryptKey(encryptedKey.CipherData.CipherValue, _certificate.GetRSAPrivateKey(), encryptedKey.EncryptionMethod.KeyAlgorithm == XmlEncRSAOAEPUrl);

        }
    }
}
