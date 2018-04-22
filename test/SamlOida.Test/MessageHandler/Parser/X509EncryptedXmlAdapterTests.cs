using SamlOida.MessageHandler.Parser;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Xunit;

namespace SamlOida.Test.MessageHandler.Parser
{
    public class X509EncryptedXmlAdapterTests
    {
        private readonly X509Certificate2 _privateCert;
        private readonly X509Certificate2 _publicCert;
        private readonly XmlDocument _doc;

        private const string EncryptedDocument = "<Test><EncryptedElement><xenc:EncryptedData xmlns:xenc=\"http://www.w3.org/2001/04/xmlenc#\" xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\" Type=\"http://www.w3.org/2001/04/xmlenc#Element\"><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#aes128-cbc\"/><dsig:KeyInfo xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\"><xenc:EncryptedKey><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#rsa-1_5\"/><xenc:CipherData><xenc:CipherValue>AyWSNYb8Nn4N1XtSQo1F36+boKuIOU0LCssejP9hVcI2+128bm5zNrmACbjdHMjd+ZKYDr+/JYOuA/abnP1cl7AN+MzemyOimi/13RuJdbwOa5ydUOouwUH3PvBKaQVRIA42NMcFOVgPs9FNxg94f2osumaFS7v4L1l8qmOHGljV0EbbOcEDEKh6MtZatc3c1csghw+Nxkqo+hsSVQaJJ0Tj27s+b0uYvb5O1517nqH40t/NfZdAVx6V81ZY4nVoF7ahvj51G0pvsJ8YxgxjBIKdnO6njYnS4sN8MgIpGTXAlkNWCldMz+9osJeRrLDAHQUQKSt4GbtnpJFzHOJZZA==</xenc:CipherValue></xenc:CipherData></xenc:EncryptedKey></dsig:KeyInfo><xenc:CipherData><xenc:CipherValue>sAKo4y7qwSU4YlZ3Anc4DFSYijN/XIAVtGHQfPzENgzU9J5x8GYYuADu/27lXvRI6ovwhP8Fwiilu6qUoVUDxq3zPCqB3PGRsusoaNl36uA=</xenc:CipherValue></xenc:CipherData></xenc:EncryptedData></EncryptedElement></Test>";
        private const string MalformedEncryptedDocument = "<Test><EncryptedElement><xenc:EncryptedData xmlns:xenc=\"http://www.w3.org/2001/04/xmlenc#\" xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\" Type=\"http://www.w3.org/2001/04/xmlenc#Element\"><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#aes128-cbc\"/><dsig:KeyInfo xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\"><xenc:EncryptedKey><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#rsa-1_5\"/><xenc:CipherData><xenc:CipherValue>AyWSNYb8Nn4N1XtSQo1F36+boKuIOU0LCssejP9hVcI2+128bm5zNrmACbjdHMjd+ZKYDr+/JYOuA/abnP1cl7AN+MzemyOimi/13RuJdbwOa5ydUOouwUH3PvBKaQVRIA42NMcFOVgPs9FNxg94f2osumaFS7v4L1l8qmOHGljV0EbbOcEDEKh6MtZatc3c1csghw+Nxkqo+hsSVQaJJ0Tj27s+b0uYvb5O1517nqH40t/NfZdAVx6V81ZY4nVoF7ahvj51G0pvsJ8YxgxjBIKdnO6njYnS4sN8MgIpGTXAlkNWCldMz+9osJeRrLDAHQUQKSt4GbtnpJFzHOJZZA==</xenc:CipherValue></xenc:CipherData></xenc:EncryptedKey></dsig:KeyInfo><xenc:CipherData></xenc:CipherData></xenc:EncryptedData></EncryptedElement></Test>";

        public X509EncryptedXmlAdapterTests()
        {
            _publicCert = new X509Certificate2(File.ReadAllBytes("PublicTestCert.cer"));
            _privateCert = new X509Certificate2(File.ReadAllBytes("PrivateTestCert.pfx"), "test");
            _doc = new XmlDocument();
            _doc.LoadXml(EncryptedDocument);
        }

        [Fact]
        public void ConstructorDoesntThrowExceptionIfCertificateIsNull()
        {
            var adapter = new X509EncryptedXmlAdapter(_doc, null);
            Assert.NotNull(adapter);
        }

        [Fact]
        public void ConstructorCreatesAdapterIfCertificateHasPrivateKey()
        {
            var adapter = new X509EncryptedXmlAdapter(_doc, _privateCert);
            Assert.NotNull(adapter);
        }

        [Fact]
        public void ConstructorThrowsExceptionIfCertificateHasNoPrivateKey()
        {
            Assert.Throws<ArgumentException>(() => new X509EncryptedXmlAdapter(_doc, _publicCert));
        }

        [Fact]
        public void DecryptShouldUseCertificateToDecryptDocument()
        {
            var adapter = new X509EncryptedXmlAdapter(_doc, _privateCert);
            adapter.DecryptDocument();

            var decryptedElement = _doc.SelectSingleNode("Test/EncryptedElement/Element");
            Assert.NotNull(decryptedElement);
            Assert.Equal("You should not see this text", decryptedElement.InnerText.Trim());
        }

        [Fact]
        public void DecryptShouldThrowExceptionIfKeyDataIsNull()
        {
            var adapter = new X509EncryptedXmlAdapter(_doc, _privateCert);
            Assert.Throws<ArgumentNullException>(() => adapter.DecryptEncryptedKey(null));
        }
    }
}
