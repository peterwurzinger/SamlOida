using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;

namespace SamlOida.MessageHandler
{
    public class SpInitiatedLogoutResponseHandler : IncomingSamlMessageHandler<object, SamlLogoutResponseMessage>
    {
        public SpInitiatedLogoutResponseHandler(ISamlMessageParser<SamlLogoutResponseMessage> messageParser, ISamlBindingStrategy binding) : base(messageParser, binding)
        {
        }

        protected internal override object HandleInternal(SamlOptions options, HttpContext httpContext, SamlLogoutResponseMessage messageContext)
        {
            throw new NotImplementedException();
        }
    }
}
