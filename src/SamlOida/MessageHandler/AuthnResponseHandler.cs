using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;

namespace SamlOida.MessageHandler
{
    public class AuthnResponseHandler : IncomingSamlMessageHandler<AuthnResultContext, SamlAuthnResponseMessage>
    {
        //TODO: HttpPostBinding isn't always sufficient
        public AuthnResponseHandler(AuthnResponseParser messageParser, HttpPostBindingHandler binding)
            : base(messageParser, binding)
        {
        }

        protected internal override void Validate(SamlOptions options, ExtractionResult extractionResult,
            SamlAuthnResponseMessage messageContext)
        {
            if ((DateTime.UtcNow - messageContext.IssueInstant) > options.IssueInstantExpiration)
                throw new SamlException("Issue instant is too long ago.");
        }

        protected internal override AuthnResultContext HandleInternal(SamlOptions options, HttpContext httpContext, SamlAuthnResponseMessage messageContext)
        {
            //TODO: Handle the Response-Message
            return new AuthnResultContext
            {
            };
        }
    }

    public class AuthnResultContext
    {
        //TODO: Needed?
    }
}
