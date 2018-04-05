using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;
using System.Security.Claims;

namespace SamlOida.MessageHandler
{
    public class AuthnResponseHandler : IncomingSamlMessageHandler<HandleRequestResult, SamlAuthnResponseMessage>
    {
        public AuthnResponseHandler(AuthnResponseParser messageParser)
            : base(messageParser)
        {
        }

        protected internal override void Validate(SamlOptions options, ExtractionResult extractionResult,
            SamlAuthnResponseMessage messageContext)
        {
            if ((DateTime.UtcNow - messageContext.IssueInstant) > options.IssueInstantExpiration)
                throw new SamlException("Issue instant is too long ago.");
        }

        protected internal override HandleRequestResult HandleInternal(SamlOptions options, HttpContext httpContext, SamlAuthnResponseMessage messageContext)
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), SamlAuthenticationDefaults.AuthenticationScheme));

            var props = new AuthenticationProperties
            {
                //TODO: Find a way to propagate this.
                //RedirectUri = relaystate
            };
            
            //TODO: Insert mapped Identity
            var authTicket = new AuthenticationTicket(principal, props, SamlAuthenticationDefaults.AuthenticationScheme);

            return HandleRequestResult.Success(authTicket);
        }
    }
}
