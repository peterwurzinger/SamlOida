using Microsoft.AspNetCore.Http;
using System.Xml;

namespace SamlOida.Binding
{
    public interface ISamlBindingStrategy
    {
        void BindMessage(XmlDocument message, HttpContext context, string target, string flowKey, SamlOptions options,
            string relayState = null);
    }
}