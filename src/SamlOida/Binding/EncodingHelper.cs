using Microsoft.AspNetCore.Http;
using SamlOida.MessageHandler;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace SamlOida.Binding
{
    public static class EncodingHelper
    {
        internal static QueryString EncodeMessage(SamlOptions options, string encoding, XmlDocument message, string samlFlowKey, string relayState)
        {
            if (encoding == SamlAuthenticationDefaults.DeflateEncoding)
                return SamlDeflateEncoding(options, message, samlFlowKey, relayState);

            throw new NotImplementedException();
        }
        
        private static QueryString SamlDeflateEncoding(SamlOptions options, XmlDocument message, string samlFlowKey, string relayState)
        {
            //See https://docs.oasis-open.org/security/saml/v2.0/saml-bindings-2.0-os.pdf for details
            var dict = new Dictionary<string,string>();

            var signatureElement = message.RemoveSignature();

            var messageBytes = message.Deflate();
            var base64Message = Convert.ToBase64String(messageBytes);
            dict.Add(samlFlowKey, base64Message);

            if (!string.IsNullOrEmpty(relayState))
                dict.Add(SamlAuthenticationDefaults.RelayStateKey, relayState);

            if (signatureElement != null)
            {
                //Construct Signature
                byte[] signature;

                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"{samlFlowKey}={WebUtility.UrlEncode(dict[samlFlowKey])}");

                if (!string.IsNullOrEmpty(relayState))
                    stringBuilder.Append($"&{SamlAuthenticationDefaults.RelayStateKey}={WebUtility.UrlEncode(relayState)}");

                dict.Add(SamlAuthenticationDefaults.SignatureAlgorithmKey, "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
                stringBuilder.Append($"&{SamlAuthenticationDefaults.SignatureAlgorithmKey}={WebUtility.UrlEncode(dict[SamlAuthenticationDefaults.SignatureAlgorithmKey])}");

                using (var rsa = options.ServiceProviderCertificate.GetRSAPrivateKey())
                {
                    var signatureData = Encoding.UTF8.GetBytes(stringBuilder.ToString());

                    signature = rsa.SignData(signatureData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }
                
                dict.Add(SamlAuthenticationDefaults.SignatureKey, Convert.ToBase64String(signature));
            }

            return QueryString.Create(dict);
        }
    }
}
