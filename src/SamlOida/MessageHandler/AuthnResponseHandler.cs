using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var principal = new ClaimsPrincipal();
            foreach (var assertion in messageContext.Assertions)
            {
                var claims = new List<Claim>();
                if (assertion.SessionIndex != null)
                    claims.Add(new Claim(SamlAuthenticationDefaults.SessionIndexClaimType, assertion.SessionIndex, ClaimValueTypes.String));

                if (assertion.SubjectNameId != null)
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, assertion.SubjectNameId));

                claims.AddRange(options.ClaimsSelector(assertion.Attributes.ToList()));
                
                principal.AddIdentity(new ClaimsIdentity(claims, assertion.Issuer));
            }

            var props = new AuthenticationProperties
            {
                //TODO: Find a way to propagate this.
                //RedirectUri = relaystate
            };
            
            var authTicket = new AuthenticationTicket(principal, props, SamlAuthenticationDefaults.AuthenticationScheme);

            return HandleRequestResult.Success(authTicket);
        }
    }
}
