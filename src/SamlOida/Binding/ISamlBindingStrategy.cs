using System.Xml;
using Microsoft.AspNetCore.Http;

namespace SamlOida.Binding
{
    public interface ISamlBindingStrategy
    {
        ExtractionResult ExtractMessage(HttpContext context);

        void SendMessage(HttpContext context, XmlDocument message, string relayState = null);
    }

    public class ExtractionResult
    {
        public XmlDocument Message { get; set; }

        public string RelayState { get; set; }
    }
}