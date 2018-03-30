using SamlOida.Model;
using System;
using System.Xml;

namespace SamlOida.MessageHandler.Parser
{
    public class LogoutResponseParser : ISamlMessageParser<SamlLogoutResponseMessage>
    {
        public SamlLogoutResponseMessage Parse(XmlDocument message)
        {
            throw new NotImplementedException();
        }
    }
}
