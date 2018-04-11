using Microsoft.AspNetCore.Http;
using System;
using System.Xml;

namespace SamlOida.Binding
{
    public class HttpRedirectBindingHandler : ISamlBindingStrategy
    {
        public void BindMessage(XmlDocument message, HttpContext context, string target, string flowKey, SamlOptions options,
            string relayState = null)
        {
            var queryString = EncodingHelper.EncodeMessage(options, SamlAuthenticationDefaults.DeflateEncoding, message, flowKey, relayState);

            var uriBuilder = new UriBuilder(target)
            {
                Query = queryString.ToString()
            };

            context.Response.Redirect(uriBuilder.ToString());
        }
    }
}