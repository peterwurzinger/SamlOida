using Microsoft.AspNetCore.Http;
using SamlOida.MessageHandler;
using System;
using System.Xml;

namespace SamlOida.Binding
{
    public class HttpRedirectBindingHandler : ISamlBindingStrategy
    {
        public ExtractionResult ExtractMessage(SamlOptions options, HttpContext context)
        {
            //TODO: Could also be SAMLResponse
            var encodedMessage = context.Request.Query[SamlAuthenticationDefaults.SamlRequestKey];

            var binaryMessage = Convert.FromBase64String(encodedMessage);
            return new ExtractionResult
            {
                Message = binaryMessage.ToXmlDocument(),
                RelayState = context.Request.Query[SamlAuthenticationDefaults.RelayStateKey]
            };
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