using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;

namespace SamlOida.MessageHandler
{
    public class IdpInitiatedLogoutRequestHandler : IncomingSamlMessageHandler<object, SamlLogoutRequestMessage>
    {
        public IdpInitiatedLogoutRequestHandler(ISamlMessageParser<SamlLogoutRequestMessage> messageParser, ISamlBindingStrategy binding) : base(messageParser, binding)
        {
        }

        protected internal override object HandleInternal(SamlOptions options, HttpContext httpContext, SamlLogoutRequestMessage messageContext)
        {
            throw new NotImplementedException();
        }
    }
}
