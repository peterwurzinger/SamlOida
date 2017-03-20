using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace SamlOida
{
    public class SamlHandler : RemoteAuthenticationHandler<SamlOptions>
    {
        private readonly SamlBindingHandler _bindingHandler;

        public SamlHandler(SamlBindingOptions bindingOptions)
        {
            switch (bindingOptions.BindingBehavior)
            {
                case SamlBindingBehavior.HttpRedirect:
                    _bindingHandler = new HttpRedirectBindingHandler(bindingOptions);
                    break;
                case SamlBindingBehavior.HttpPost:
                    _bindingHandler = new HttpPostBindingHandler(bindingOptions);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bindingOptions.BindingBehavior));
            }
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            var authnRequest = AuthnRequestBuilder.Build(Options.SamlBindingOptions.IdentityProviderSignOnUrl, BuildRedirectUri(Options.CallbackPath), Options.ServiceProviderEntityId);

            var encodedAuthnRequest = WebUtility.UrlEncode(Convert.ToBase64String(authnRequest.Deflate()));
            _bindingHandler.HandleAuthnRequestAsync(Response, encodedAuthnRequest, BuildRedirectUri(OriginalPath));
            
            return Task.FromResult(true);
        }

        protected override Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
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
                return Task.FromResult(AuthenticateResult.Fail(ex));
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
                return Task.FromResult(AuthenticateResult.Fail(parseEx));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Task.FromResult(AuthenticateResult.Fail(ex));
            }

            //TODO: Make IdentityMapper replaceable to be able to e.g. map custom attributes
            var claims = PvpIdentityMapper.Map(result);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SamlDefaults.AuthenticationScheme));

            var props = new AuthenticationProperties
            {
                RedirectUri = relaystate
            };
            var authTicket = new AuthenticationTicket(principal, props, SamlDefaults.AuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(authTicket));
        }
    }
}
