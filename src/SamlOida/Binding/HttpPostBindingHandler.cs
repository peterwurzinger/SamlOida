using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Xml;

namespace SamlOida.Binding
{
    public class HttpPostBindingHandler : ISamlBindingStrategy
    {
        public void BindMessage(XmlDocument message, HttpContext context, Uri target, string flowKey, SamlOptions options,
            string relayState = null)
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
            
            builder.Append($"<input type='hidden' name='{flowKey}' value='{encodedMessage}'/>");

            if (!string.IsNullOrEmpty(relayState))
                builder.Append($"<input type='hidden' name='{SamlAuthenticationDefaults.RelayStateKey}' value='{HtmlEncoder.Default.Encode(relayState)}'/>");

            builder.Append("<input type='submit' value='Send'/>");
            builder.Append("</form></body></html>");

            context.Response.ContentType = "text/html";
            context.Response.WriteAsync(builder.ToString());
        }
    }
}