using Microsoft.AspNetCore.Http;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;

namespace SamlOida.MessageHandler
{
    public class IdpInitiatedLogoutRequestHandler : IncomingSamlMessageHandler<SamlLogoutResponseMessage, SamlLogoutRequestMessage>
    {
        public IdpInitiatedLogoutRequestHandler(LogoutRequestParser messageParser) : base(messageParser)
        {
        }

        protected internal override SamlLogoutResponseMessage HandleInternal(SamlOptions options, HttpContext httpContext, SamlLogoutRequestMessage messageContext)
        {
            throw new NotImplementedException();
        }
    }
}
