using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;

namespace SamlOida.MessageHandler
{
    public class IdpInitiatedLogoutRequestHandler : IncomingSamlMessageHandler<SamlLogoutResponseMessage, SamlLogoutRequestMessage>
    {
        //TODO: Binding
        public IdpInitiatedLogoutRequestHandler(LogoutRequestParser messageParser, HttpRedirectBindingHandler binding) : base(messageParser, binding)
        {
        }

        protected internal override SamlLogoutResponseMessage HandleInternal(SamlOptions options, HttpContext httpContext, SamlLogoutRequestMessage messageContext)
        {
            throw new NotImplementedException();
        }
    }
}
