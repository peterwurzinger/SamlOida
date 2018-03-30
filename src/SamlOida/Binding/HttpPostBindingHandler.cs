using Microsoft.AspNetCore.Http;
using SamlOida.MessageHandler;
using System;
using System.Net;
using System.Text;
using System.Xml;

namespace SamlOida.Binding
{
    public class HttpPostBindingHandler : ISamlBindingStrategy
    {
        public ExtractionResult ExtractMessage(SamlOptions options, HttpContext context)
        {
            //TODO: Could also be SAMLRequest!
            var encodedMessage = context.Request.Form[SamlAuthenticationDefaults.SamlResponseKey];

            var binaryMessage = Convert.FromBase64String(encodedMessage);
            return new ExtractionResult
            {
                Message = binaryMessage.ToXmlDocument(),
                RelayState = context.Request.Form[SamlAuthenticationDefaults.RelayStateKey]
            };
        }

        public void SendMessage(SamlOptions options, HttpContext context, XmlDocument message, Uri target, string relayState = null)
        {
            var encodedMessage = EncodingHelper.EncodeMessage(message);

            var builder = new StringBuilder();
            builder.Append($"<html><body><form action='{target.AbsoluteUri}'>");

            //TODO: Could also be SAMLResponse!
            builder.Append($"<input type='hidden' name='{SamlAuthenticationDefaults.SamlRequestKey}' value='{encodedMessage}'/>");

            if (!string.IsNullOrEmpty(relayState))
                builder.Append($"<input type='hidden' name='{SamlAuthenticationDefaults.RelayStateKey}' value='{WebUtility.UrlEncode(relayState)}'/>");

            builder.Append("<input type='submit' value='Send'/>");
            builder.Append("</form></body></html>");

            context.Response.ContentType = "text/html";
            context.Response.WriteAsync(builder.ToString());
        }
    }
}