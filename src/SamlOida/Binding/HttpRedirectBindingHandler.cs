using Microsoft.AspNetCore.Http;
using SamlOida.MessageHandler;
using System;
using System.Xml;

namespace SamlOida.Binding
{
    public class HttpRedirectBindingHandler : ISamlBindingStrategy
    {
        public ExtractionResult ExtractMessage(HttpContext context)
        {
            if (!context.Request.Query.TryGetValue(SamlAuthenticationDefaults.SamlResponseKey, out var messageString))
                context.Request.Query.TryGetValue(SamlAuthenticationDefaults.SamlRequestKey, out messageString);

            if (string.IsNullOrEmpty(messageString))
                throw new SamlException("Request contains no SAML2 payload.");

            var result = new ExtractionResult();

            var binaryMessage = Convert.FromBase64String(messageString);
            result.Message = binaryMessage.ToXmlDocument();

            if (context.Request.Query.TryGetValue(SamlAuthenticationDefaults.RelayStateKey, out var relayState))
                result.RelayState = relayState;

            if (context.Request.Query.TryGetValue(SamlAuthenticationDefaults.SignatureKey, out var signature))
                result.Signature = Convert.FromBase64String(signature);

            if (context.Request.Query.TryGetValue(SamlAuthenticationDefaults.SignatureAlgorithmKey, out var sigAlg))
                result.SignatureAlgorithm = sigAlg;

            return result;
        }

        public void SendMessage(SamlOptions options, HttpContext context, XmlDocument message, Uri target, string relayState = null)
        {
            //TODO: Obtain encoding to use from options?
            var encoding = string.Empty;
            if (string.IsNullOrEmpty(encoding))
                encoding = SamlAuthenticationDefaults.DeflateEncoding;

            //TODO: Could also be SAMLResponse!
            var queryString = EncodingHelper.EncodeMessage(options, encoding, message, SamlAuthenticationDefaults.SamlRequestKey, relayState);

            var uriBuilder = new UriBuilder(target)
            {
                Query = queryString.ToString()
            };

            context.Response.Redirect(uriBuilder.Uri.AbsoluteUri);
        }
    }
}