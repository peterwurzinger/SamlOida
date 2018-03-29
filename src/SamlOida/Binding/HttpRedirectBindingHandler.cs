using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using SamlOida.MessageHandler;
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using Microsoft.Extensions.Options;

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

        public void SendMessage(HttpContext context, XmlDocument message, string relayState = null)
        {
            var encodedMessage = EncodingHelper.EncodeMessage(message);
            var dict = new Dictionary<string, string>
            {
                { SamlAuthenticationDefaults.SamlRequestKey, encodedMessage }
            };

            if (!string.IsNullOrEmpty(relayState))
                dict.Add(SamlAuthenticationDefaults.RelayStateKey, WebUtility.UrlEncode(relayState));

            //TODO: Extract IdP-SignonUrl!
            var query = QueryHelpers.AddQueryString(_samloptions.Value.IdentityProviderSignOnUrl.ToString(), dict);


            context.Response.Redirect(query);
        }
    }
}