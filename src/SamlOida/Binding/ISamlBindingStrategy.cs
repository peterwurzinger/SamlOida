using Microsoft.AspNetCore.Http;
using System;
using System.Xml;

namespace SamlOida.Binding
{
    public interface ISamlBindingStrategy
    {
        ExtractionResult ExtractMessage(HttpContext context);

        void SendMessage(HttpContext context, XmlDocument message, Uri target, string relayState = null);
    }

    public class ExtractionResult
    {
        public XmlDocument Message { get; set; }

        public string RelayState { get; set; }
    }
}