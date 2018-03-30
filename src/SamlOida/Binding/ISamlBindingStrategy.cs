using Microsoft.AspNetCore.Http;
using System;
using System.Xml;

namespace SamlOida.Binding
{
    public interface ISamlBindingStrategy
    {
        ExtractionResult ExtractMessage(SamlOptions options, HttpContext context);

        void SendMessage(SamlOptions options, HttpContext context, XmlDocument message, Uri target, string relayState = null);
    }

    public class ExtractionResult
    {
        public XmlDocument Message { get; set; }

        public string RelayState { get; set; }
    }
}