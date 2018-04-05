using Microsoft.AspNetCore.Http;
using System;
using System.Xml;

namespace SamlOida.Binding
{
    public class HttpRedirectBindingHandler : ISamlBindingStrategy
    {
        public void BindMessage(XmlDocument message, HttpContext context, Uri target, SamlOptions options,
            string relayState = null)
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