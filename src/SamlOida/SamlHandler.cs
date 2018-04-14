using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SamlOida.MessageHandler;
using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System;
using System.Linq;
using System.Security.Claims;
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

        /// <inheritdoc />
        public override Task<bool> HandleRequestAsync()
        {
            if (Options.LogoutPath == Request.Path)
            {
                //Wurzinger: Seems a bit hacky to me, but it works...
                if (HttpMethods.IsGet(Context.Request.Method) &&
                    Context.Request.Query.ContainsKey(SamlAuthenticationDefaults.SamlRequestKey)
                    || HttpMethods.IsPost(Context.Request.Method) &&
                    Context.Request.Form.ContainsKey(SamlAuthenticationDefaults.SamlRequestKey))
                {

                    var responseMessage = _idpInitiatedLogoutRequestHandler.Handle(Options, Context);
                    _idpInitiatedLogoutResponseHandler.Handle(Options, Context, responseMessage, Options.IdentityProviderSignOnUrl);
                    return Task.FromResult(true);
                }

                _spInitiatedLogoutResponseHandler.Handle(Options, Context);
                return Task.FromResult(true);

            }

            return base.HandleRequestAsync();
        }


        /// <inheritdoc />
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var context = new SamlAuthnRequestMessage
            {
                AssertionConsumerServiceUrl = BuildRedirectUri(Options.CallbackPath),
                Issuer = Options.ServiceProviderEntityId,
                Destination = Options.IdentityProviderSignOnUrl
            };
            _authnRequestHandler.Handle(Options, Request.HttpContext, context, Options.IdentityProviderSignOnUrl, BuildRedirectUri(OriginalPath));

            return Task.CompletedTask;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public Task SignOutAsync(AuthenticationProperties properties)
        {
            //SP-initiated Signout
            var logoutRequestMessage = new SamlLogoutRequestMessage
            {
                Issuer = Options.ServiceProviderEntityId,
                Destination = Options.IdentityProviderLogOutUrl,
                NameId = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                SessionIndex = Context.User.Claims.FirstOrDefault(c => c.Type == SamlAuthenticationDefaults.SessionIndexClaimType)?.Value
            };
            _spInitiatedLogoutRequestHandler.Handle(Options, Context, logoutRequestMessage,
                Options.IdentityProviderLogOutUrl, properties?.RedirectUri ?? Request.Path);
            return Task.CompletedTask;
        }
    }
}
