using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;

namespace SamlOida.MessageHandler
{
    public class IdpInitiatedLogoutRequestHandler : IncomingSamlMessageHandler<SamlLogoutResponseMessage, SamlLogoutRequestMessage>
    {
        public IdpInitiatedLogoutRequestHandler(LogoutRequestParser messageParser) : base(messageParser)
        {
        }

        protected internal override SamlLogoutResponseMessage HandleInternal(SamlOptions options, HttpContext httpContext, SamlLogoutRequestMessage messageContext, string relayState = null)
        {
            httpContext.SignOutAsync(options.SignInScheme).GetAwaiter().GetResult();
            return new SamlLogoutResponseMessage
            {
                Destination = options.IdentityProviderLogOutUrl,
                Success = true,
                Issuer = options.ServiceProviderEntityId,
                InResponseTo = messageContext.Id
            };
        }
    }
}
