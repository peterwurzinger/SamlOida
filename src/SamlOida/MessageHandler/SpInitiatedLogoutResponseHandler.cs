using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;

namespace SamlOida.MessageHandler
{
    public class SpInitiatedLogoutResponseHandler : IncomingSamlMessageHandler<object, SamlLogoutResponseMessage>
    {
        public SpInitiatedLogoutResponseHandler(LogoutResponseParser messageParser) : base(messageParser)
        {
        }

        protected internal override object HandleInternal(SamlOptions options, HttpContext httpContext, SamlLogoutResponseMessage messageContext, string relayState = null)
        {
            if (messageContext.Success)
                httpContext.SignOutAsync(options.SignInScheme).Wait();

            if (relayState != null)
                httpContext.Response.Redirect(relayState);

            return null;
        }
    }
}
