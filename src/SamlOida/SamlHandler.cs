using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SamlOida.MessageHandler;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SamlOida
{
    public class SamlHandler : RemoteAuthenticationHandler<SamlOptions>, IAuthenticationSignOutHandler
    {
        private readonly AuthnRequestHandler _authnRequestHandler;
        private readonly AuthnResponseHandler _authnResponseHandler;

        public SamlHandler(IOptionsMonitor<SamlOptions> options, ILoggerFactory loggerFactory, UrlEncoder urlEncoder, ISystemClock clock,
            AuthnRequestHandler authnRequestHandler, AuthnResponseHandler authnResponseHandler)
            : base(options, loggerFactory, urlEncoder, clock)
        {
            _authnRequestHandler = authnRequestHandler ?? throw new ArgumentNullException(nameof(authnRequestHandler));
            _authnResponseHandler = authnResponseHandler ?? throw new ArgumentNullException(nameof(authnResponseHandler));
        }

        public override Task<bool> ShouldHandleRequestAsync()
        {
            //TODO: Check if Check if IdP-Initiated Signout
            //TODO: Check if SP-Initiated Callback
            return base.ShouldHandleRequestAsync();
        }

        public override Task<bool> HandleRequestAsync()
        {
            //TODO: Check if IdP-Initiated Signout, Instantiate IncomingLogoutRequestHandler

            //TODO: Check if SP-Initiated Callback, Instantiate IncomingLogoutResponseHandler
            return base.HandleRequestAsync();
        }


        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var context = new SamlAuthnRequestMessage
            {
                AssertionConsumerServiceUrl = BuildRedirectUri(Options.CallbackPath),
                Issuer = "Ich selber",
                Destination = Options.IdentityProviderSignOnUrl.AbsoluteUri
            };
            _authnRequestHandler.Handle(Request.HttpContext, context, Options.IdentityProviderSignOnUrl, BuildRedirectUri(OriginalPath));

            return Task.CompletedTask;
        }

        protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            //TODO: Instantiate AuthnResponseHandler
            AuthnResultContext result;

            //Authenticate user based on SamlResponse
            var relaystate = BuildRedirectUri("/");

            //TODO: Check if RelayState references a local ressource (?)

            try
            {
                result = _authnResponseHandler.Handle(Request.HttpContext);
            }
            catch (ParsingException parseEx)
            {
                //HTTP 400 = Bad Request
                Response.StatusCode = 400;
                return Task.FromResult(HandleRequestResult.Fail(parseEx));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Task.FromResult(HandleRequestResult.Fail(ex));
            }

            //TODO: Map Identity


            //var claims = PvpIdentityMapper.Map(result);

            //TODO: Insert mapped Identity
            var principal = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), SamlAuthenticationDefaults.AuthenticationScheme));

            var props = new AuthenticationProperties
            {
                RedirectUri = relaystate
            };
            var authTicket = new AuthenticationTicket(principal, props, SamlAuthenticationDefaults.AuthenticationScheme);

            return Task.FromResult(HandleRequestResult.Success(authTicket));
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            //TODO: Instantiate OutgoingLogoutRequestHandler

            return Task.CompletedTask;
        }
    }
}
