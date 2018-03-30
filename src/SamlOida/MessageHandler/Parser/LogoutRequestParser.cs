using SamlOida.Model;
using System;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public class LogoutRequestParser : ISamlMessageParser<SamlLogoutRequestMessage>
    {
        public SamlLogoutRequestMessage Parse(XmlDocument message)
        {
            throw new NotImplementedException();
        }
    }
}
