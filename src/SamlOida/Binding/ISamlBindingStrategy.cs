using Microsoft.AspNetCore.Http;
using System;
using System.Xml;

namespace SamlOida.Binding
{
    public interface ISamlBindingStrategy
    {
        ExtractionResult ExtractMessage(HttpContext context);

        void SendMessage(SamlOptions options, HttpContext context, XmlDocument message, Uri target, string relayState = null);
    }
}