using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Xml;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;

namespace SamlOida
{
    public class SamlHandler : RemoteAuthenticationHandler<SamlOptions>
    {
        private readonly SamlBindingHandler _bindingHandler;

        public SamlHandler(IOptionsMonitor<SamlOptions> options, IOptionsMonitor<SamlBindingOptions> bindingOptions, ILoggerFactory loggerFactory, UrlEncoder urlEncoder, ISystemClock clock) 
            : base(options, loggerFactory, urlEncoder, clock)
        {
            switch (bindingOptions.CurrentValue.BindingBehavior)
            {
                case SamlBindingBehavior.HttpRedirect:
                    _bindingHandler = new HttpRedirectBindingHandler(bindingOptions.CurrentValue);
                    break;
                case SamlBindingBehavior.HttpPost:
                    _bindingHandler = new HttpPostBindingHandler(bindingOptions.CurrentValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bindingOptions.CurrentValue.BindingBehavior));
            }
        }

        public override Task<bool> HandleRequestAsync()
        {
            //TODO: Check if IdP-Initiated Signout

            //TODO: Check if SP-Initiated Callback
            return base.HandleRequestAsync();
        }
        

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var authnRequest = AuthnRequestBuilder.Build(Options.SamlBindingOptions.IdentityProviderSignOnUrl, BuildRedirectUri(Options.CallbackPath), Options.ServiceProviderEntityId);

            var encodedAuthnRequest = WebUtility.UrlEncode(Convert.ToBase64String(authnRequest.Deflate()));
            return _bindingHandler.HandleAuthnRequestAsync(Response, encodedAuthnRequest, BuildRedirectUri(OriginalPath));
        }

        protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            //Authenticate user based on SamlResponse
            string samlResponse;
            var relaystate = BuildRedirectUri("/");

            if (Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(Request.Query[SamlDefaults.SamlResponseQueryStringKey]))
                    throw new ArgumentException("SAMLResponse is empty");
                samlResponse = Request.Query[SamlDefaults.SamlResponseQueryStringKey];

                if (!string.IsNullOrEmpty(Request.Query[SamlDefaults.RelayStateQueryStringKey]))
                    relaystate = Request.Query[SamlDefaults.RelayStateQueryStringKey];
            }
            else if (Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(Request.Form[SamlDefaults.SamlResponseQueryStringKey]))
                    throw new ArgumentException("SAMLResponse is empty");
                samlResponse = Request.Form[SamlDefaults.SamlResponseQueryStringKey];

                if (!string.IsNullOrEmpty(Request.Form[SamlDefaults.RelayStateQueryStringKey]))
                    relaystate = Request.Form[SamlDefaults.RelayStateQueryStringKey];
            }
            else
            {
                throw new InvalidOperationException($"Request method {Request.Method} is not supported");
            }

            //TODO: Check if RelayState references a local ressource (?)

            XmlDocument doc;
            try
            {
                var binarySamlReponse = Convert.FromBase64String(samlResponse);
                doc = binarySamlReponse.ToXmlDocument();
            }
            catch (Exception ex)
            {
                return Task.FromResult(HandleRequestResult.Fail(ex));
            }

            ResponseParsingResult result;
            try
            {
                var parser = new ResponseParser(doc, Options);
                result = parser.Parse();
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

            //TODO: Make IdentityMapper replaceable to be able to e.g. map custom attributes
            var claims = PvpIdentityMapper.Map(result);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SamlDefaults.AuthenticationScheme));

            var props = new AuthenticationProperties
            {
                RedirectUri = relaystate
            };
            var authTicket = new AuthenticationTicket(principal, props, SamlDefaults.AuthenticationScheme);

            return Task.FromResult(HandleRequestResult.Success(authTicket));
        }
    }
}
