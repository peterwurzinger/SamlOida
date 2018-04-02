using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SamlOida.MessageHandler;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SamlOida
{
    public class SamlHandler : RemoteAuthenticationHandler<SamlOptions>, IAuthenticationSignOutHandler
    {
        private readonly AuthnRequestHandler _authnRequestHandler;
        private readonly AuthnResponseHandler _authnResponseHandler;
        private readonly SpInitiatedLogoutRequestHandler _spInitiatedLogoutRequestHandler;
        private readonly SpInitiatedLogoutResponseHandler _spInitiatedLogoutResponseHandler;
        private readonly IdpInitiatedLogoutRequestHandler _idpInitiatedLogoutRequestHandler;
        private readonly IdpInitiatedLogoutResponseHandler _idpInitiatedLogoutResponseHandler;

        public SamlHandler(IOptionsMonitor<SamlOptions> options, ILoggerFactory loggerFactory, UrlEncoder urlEncoder, ISystemClock clock,
            AuthnRequestHandler authnRequestHandler, AuthnResponseHandler authnResponseHandler,
            SpInitiatedLogoutRequestHandler spInitiatedLogoutRequestHandler, SpInitiatedLogoutResponseHandler spInitiatedLogoutResponseHandler,
            IdpInitiatedLogoutRequestHandler idpInitiatedLogoutRequestHandler, IdpInitiatedLogoutResponseHandler idpInitiatedLogoutResponseHandler)
            : base(options, loggerFactory, urlEncoder, clock)
        {
            _authnRequestHandler = authnRequestHandler ?? throw new ArgumentNullException(nameof(authnRequestHandler));
            _authnResponseHandler = authnResponseHandler ?? throw new ArgumentNullException(nameof(authnResponseHandler));
            _spInitiatedLogoutRequestHandler = spInitiatedLogoutRequestHandler ?? throw new ArgumentNullException(nameof(spInitiatedLogoutRequestHandler));
            _spInitiatedLogoutResponseHandler = spInitiatedLogoutResponseHandler ?? throw new ArgumentNullException(nameof(spInitiatedLogoutResponseHandler));
            _idpInitiatedLogoutRequestHandler = idpInitiatedLogoutRequestHandler ?? throw new ArgumentNullException(nameof(idpInitiatedLogoutRequestHandler));
            _idpInitiatedLogoutResponseHandler = idpInitiatedLogoutResponseHandler ?? throw new ArgumentNullException(nameof(idpInitiatedLogoutResponseHandler));
        }

        public override Task<bool> HandleRequestAsync()
        {
            //Callback to SpInitiatedSignout
            if (Options.SignoutCallbackPath == Request.Path)
            {
                _spInitiatedLogoutResponseHandler.Handle(Options, Context);
                return Task.FromResult(true);
            }

            //IdpInitiatedSignout
            if (Options.SignoutPath == Request.Path)
            {
                var responseMessage =  _idpInitiatedLogoutRequestHandler.Handle(Options, Context);
                _idpInitiatedLogoutResponseHandler.Handle(Options, Context, responseMessage, Options.IdentityProviderSignOnUrl);
                return Task.FromResult(true);
            }

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
            _authnRequestHandler.Handle(Options, Request.HttpContext, context, Options.IdentityProviderSignOnUrl, BuildRedirectUri(OriginalPath));

            return Task.CompletedTask;
        }

        protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            //Authenticate user based on SamlResponse
            var relaystate = BuildRedirectUri("/");

            try
            {
                return Task.FromResult(_authnResponseHandler.Handle(Options, Request.HttpContext));
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

        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            _spInitiatedLogoutRequestHandler.Handle(Options, Context, new SamlLogoutRequestMessage(), Options.IdentityProviderSignOutUrl, Context.Request.Path);

            return Task.CompletedTask;
        }
    }
}
