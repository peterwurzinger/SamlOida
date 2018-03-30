using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SamlOida.MessageHandler;
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace SamlOida.Binding
{
    public class HttpRedirectBindingHandler : ISamlBindingStrategy
    {
        private readonly IOptions<SamlOptions> _samloptions;

        public HttpRedirectBindingHandler(IOptions<SamlOptions> samloptions)
        {
            _samloptions = samloptions ?? throw new ArgumentNullException(nameof(samloptions));
        }

        public ExtractionResult ExtractMessage(HttpContext context)
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

        public void SendMessage(HttpContext context, XmlDocument message, Uri target, string relayState = null)
        {
            var encodedMessage = EncodingHelper.EncodeMessage(message);
            //TODO: Could also be SAMLResponse!
            var dict = new Dictionary<string, string>
            {
                { SamlAuthenticationDefaults.SamlRequestKey, encodedMessage }
            };

            if (!string.IsNullOrEmpty(relayState))
                dict.Add(SamlAuthenticationDefaults.RelayStateKey, WebUtility.UrlEncode(relayState));

            var query = QueryHelpers.AddQueryString(target.AbsoluteUri, dict);


            context.Response.Redirect(query);
        }
    }
}