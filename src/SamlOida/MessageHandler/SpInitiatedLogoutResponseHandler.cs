using Microsoft.AspNetCore.Http;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;

namespace SamlOida.MessageHandler
{
    public class SpInitiatedLogoutResponseHandler : IncomingSamlMessageHandler<object, SamlLogoutResponseMessage>
    {
        public SpInitiatedLogoutResponseHandler(LogoutResponseParser messageParser) : base(messageParser)
        {
        }

        protected internal override object HandleInternal(SamlOptions options, HttpContext httpContext, SamlLogoutResponseMessage messageContext)
        {
            throw new NotImplementedException();
        }
    }
}
