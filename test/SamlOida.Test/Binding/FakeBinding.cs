using System.Xml;
using Microsoft.AspNetCore.Http;
using SamlOida.Binding;

namespace SamlOida.Test.Binding
{
    internal class FakeBinding : ISamlBindingStrategy
    {
        public bool BindMessageCalled { get; private set; }
        public void BindMessage(XmlDocument message, HttpContext context, string target, string flowKey, SamlOptions options,
            string relayState = null)
        {
            BindMessageCalled = true;
        }
    }
}