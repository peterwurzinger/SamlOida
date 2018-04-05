using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SamlOida.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamlOida.Binding
{
    internal static class ExtractionHelper
    {
        internal static ExtractionResult ExtractHttpPost(IFormCollection requestForm)
        {
            var messageString = GetSamlPayload(requestForm);

            var binaryMessage = Convert.FromBase64String(messageString);
            return new ExtractionResult
            {
                Message = binaryMessage.ToXmlDocument(),
                RelayState = requestForm[SamlAuthenticationDefaults.RelayStateKey]
            };
        }

        internal static ExtractionResult ExtractHttpRedirect(IQueryCollection requestQuery)
        {
            var messageString = GetSamlPayload(requestQuery);

            var result = new ExtractionResult();

            var binaryMessage = Convert.FromBase64String(messageString);
            result.Message = binaryMessage.ToXmlDocument();

            if (requestQuery.TryGetValue(SamlAuthenticationDefaults.RelayStateKey, out var relayState))
                result.RelayState = relayState;

            if (requestQuery.TryGetValue(SamlAuthenticationDefaults.SignatureKey, out var signature))
                result.Signature = Convert.FromBase64String(signature);

            if (requestQuery.TryGetValue(SamlAuthenticationDefaults.SignatureAlgorithmKey, out var sigAlg))
                result.SignatureAlgorithm = sigAlg;

            return result;
        }

        private static string GetSamlPayload(IEnumerable<KeyValuePair<string, StringValues>> requestDictionary)
        {
            var dict = requestDictionary.ToDictionary(f => f.Key, v => v.Value);

            if (!dict.TryGetValue(SamlAuthenticationDefaults.SamlResponseKey, out var messageString))
                dict.TryGetValue(SamlAuthenticationDefaults.SamlRequestKey, out messageString);

            if (string.IsNullOrEmpty(messageString))
                throw new SamlException("Request contains no SAML2 payload.");

            return messageString;
        }
    }
}