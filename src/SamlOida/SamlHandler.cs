using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace SamlOida
{
    public class SamlHandler : RemoteAuthenticationHandler<SamlOptions>
    {
        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            var result = AuthnRequestBuilder.Build(Options.LogOnUrl, BuildRedirectUri(Options.CallbackPath), Options.Issuer);

            var base64 = WebUtility.UrlEncode(Convert.ToBase64String(result.Deflate()));

            //Redirect to IdP, SAML2 GET Profile
            Response.Redirect($"{Options.LogOnUrl}?{SamlDefaults.SamlRequestQueryStringKey}={base64}&{SamlDefaults.RelayStateQueryStringKey}={WebUtility.UrlEncode(BuildRedirectUri(OriginalPath))}");
            
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
            } else if (Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(Request.Form[SamlDefaults.SamlResponseQueryStringKey]))
                    throw new ArgumentException("SAMLResponse is empty");
                samlResponse = Request.Form[SamlDefaults.SamlResponseQueryStringKey];

                if (!string.IsNullOrEmpty(Request.Form[SamlDefaults.RelayStateQueryStringKey]))
                    relaystate = Request.Form[SamlDefaults.RelayStateQueryStringKey];
            } else {
                throw new InvalidOperationException();
            }

            //TODO: Check if RelayState references a local ressource (?)

            //TODO: URL-Encode/Decode!!
            //var doc = Convert.FromBase64String(samlResponse).InflateToXmlDocument();
            var doc = new XmlDocument();
            doc.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(samlResponse)));

            ResponseParsingResult result;
            try
            {
                var parser = new ResponseParser(doc);
                result = parser.Parse();
            }
            catch (ParsingException parseEx)
            {
                //HTTP400 = Bad Request
                Response.StatusCode = 400;
                return Task.FromResult(AuthenticateResult.Fail(parseEx));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Task.FromResult(AuthenticateResult.Fail(ex));
            }

            //TODO: Make IdentityMapper replaceable to be able to e.g. map custom attributes
            var claims =  PvpIdentityMapper.Map(result);
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
