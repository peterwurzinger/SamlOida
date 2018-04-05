using Microsoft.AspNetCore.Http;
using SamlOida.MessageHandler;
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Xml;

namespace SamlOida.Binding
{
    public class HttpPostBindingHandler : ISamlBindingStrategy
    {
        public ExtractionResult ExtractMessage(HttpContext context)
        {
            if (!context.Request.Form.TryGetValue(SamlAuthenticationDefaults.SamlResponseKey, out var messageString))
                context.Request.Form.TryGetValue(SamlAuthenticationDefaults.SamlRequestKey, out messageString);

            if (string.IsNullOrEmpty(messageString))
                throw new SamlException("Request contains no SAML2 payload.");

            var binaryMessage = Convert.FromBase64String(messageString);
            return new ExtractionResult
            {
                Message = binaryMessage.ToXmlDocument(),
                RelayState = context.Request.Form[SamlAuthenticationDefaults.RelayStateKey]
            };
        }

        public void SendMessage(SamlOptions options, HttpContext context, XmlDocument message, Uri target, string relayState = null)
        {
            byte[] binaryMessage;
            using (var stream = new MemoryStream())
            {
                message.Save(stream);

                binaryMessage = stream.ToArray();
            }
            var encodedMessage = Convert.ToBase64String(binaryMessage);
            
            var builder = new StringBuilder();
            builder.Append($"<html><body><form action='{target.AbsoluteUri}' method='POST'>");

            //TODO: Could also be SAMLResponse!
            builder.Append($"<input type='hidden' name='{SamlAuthenticationDefaults.SamlRequestKey}' value='{encodedMessage}'/>");

            if (!string.IsNullOrEmpty(relayState))
                builder.Append($"<input type='hidden' name='{SamlAuthenticationDefaults.RelayStateKey}' value='{HtmlEncoder.Default.Encode(relayState)}'/>");

            builder.Append("<input type='submit' value='Send'/>");
            builder.Append("</form></body></html>");

            context.Response.ContentType = "text/html";
            context.Response.WriteAsync(builder.ToString());
        }
    }
}