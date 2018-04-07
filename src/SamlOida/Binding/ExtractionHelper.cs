using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SamlOida.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace SamlOida.Binding
{
    internal static class ExtractionHelper
    {
        internal static ExtractionResult ExtractHttpPost(IFormCollection requestForm, X509Certificate2 idpCertificate)
        {
            var payload = GetSamlPayload(requestForm);

            var message = Convert.FromBase64String(payload.Item2).ToXmlDocument();

            var result = new ExtractionResult
            {
                Message = message,
                RelayState = requestForm[SamlAuthenticationDefaults.RelayStateKey]
            };

            var signatureNode = message.RemoveSignature();
            if (signatureNode == null || idpCertificate == null)
                return result;

            var signedXml = new SignedXml(message);

            signedXml.LoadXml((XmlElement)signatureNode);
            result.HasValidSignature = signedXml.CheckSignature(idpCertificate, true);

            return result;
        }

        internal static ExtractionResult ExtractHttpRedirect(IQueryCollection requestQuery, X509Certificate2 idpCertificate)
        {
            var payload = GetSamlPayload(requestQuery);

            var result = new ExtractionResult();

            var binaryMessage = Convert.FromBase64String(payload.Item2);
            result.Message = binaryMessage.ToXmlDocument();

            if (requestQuery.TryGetValue(SamlAuthenticationDefaults.RelayStateKey, out var relayState))
                result.RelayState = relayState;

            if (!requestQuery.TryGetValue(SamlAuthenticationDefaults.SignatureKey, out var signatureString)
                || !requestQuery.TryGetValue(SamlAuthenticationDefaults.SignatureAlgorithmKey, out var sigAlg)
                || idpCertificate == null)
                return result;

            var signature = Convert.FromBase64String(signatureString);

            var signatureDescription = (SignatureDescription)CryptoConfig.CreateFromName(sigAlg);
            if (signatureDescription == null)
                throw new SamlException($"Signature Algorithm '{sigAlg}' is not supported.");

            var signedSequence = new StringBuilder();
            signedSequence.Append($"{payload.Item1}={payload.Item2}&");

            if (result.RelayState != null)
                signedSequence.Append($"RelayState={result.RelayState}&");

            signedSequence.Append($"SigAlg={sigAlg}");

            var hashAlgorithm = signatureDescription.CreateDigest();
            hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(signedSequence.ToString()));

            result.HasValidSignature = signatureDescription.CreateDeformatter(idpCertificate.PublicKey.Key).VerifySignature(hashAlgorithm, signature);

            return result;
        }

        private static Tuple<string, string> GetSamlPayload(IEnumerable<KeyValuePair<string, StringValues>> requestDictionary)
        {
            var dict = requestDictionary.ToDictionary(f => f.Key, v => v.Value);
            var flowKey = string.Empty;

            if (dict.TryGetValue(SamlAuthenticationDefaults.SamlResponseKey, out var messageString))
            {
                flowKey = SamlAuthenticationDefaults.SamlResponseKey;
            }
            else if (dict.TryGetValue(SamlAuthenticationDefaults.SamlRequestKey, out messageString))
            {
                flowKey = SamlAuthenticationDefaults.SamlRequestKey;
            }

            if (string.IsNullOrEmpty(messageString))
                throw new SamlException("Request contains no SAML2 payload.");

            return Tuple.Create(flowKey, (string)messageString);
        }
    }
}