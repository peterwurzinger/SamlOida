using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace SamlOida
{
    public class SamlHandler : RemoteAuthenticationHandler<SamlOptions>
    {
        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            var result = AuthnRequestBuilder.Build(BuildRedirectUri(Options.CallbackPath));

            var base64 = WebUtility.UrlEncode(Convert.ToBase64String(result.Deflate()));

            //Redirect to IdP, SAML2 GET Profile
            Response.Redirect($"{Options.LogOnUrl}?SAMLRequest={base64}&RelayState={WebUtility.UrlEncode(BuildRedirectUri(OriginalPath))}");

            //Write XML out to Response
            //var bytes = Encoding.UTF8.GetBytes(result.InnerXml);
            //Response.ContentType = "text/xml";
            //Response.StatusCode = 200;
            //Response.Body.WriteAsync(bytes, 0, bytes.Length);

            return Task.FromResult(true);
        }

        protected override Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            //Authenticate user based on SamlResponse
            if (string.IsNullOrEmpty(Request.Query["SamlResponse"]))
                throw new ArgumentException("SamlResponse");

            if (string.IsNullOrEmpty(Request.Query["RelayState"]))
                throw new ArgumentException("RelayState");

            //TODO: Check if RelayState references a local ressource (?)

            var doc = Convert.FromBase64String(Request.Query["SamlResponse"]).InflateToXmlDocument();
            var parser = new ResponseParser(doc);
            var result = parser.Parse();

            var principal = new ClaimsPrincipal(result.Identity);

            var props = new AuthenticationProperties
            {
                RedirectUri = Request.Query["RelayState"]
            };
            var authTicket = new AuthenticationTicket(principal, props, SamlDefaults.AuthenticationScheme);
            
            return Task.FromResult(AuthenticateResult.Success(authTicket));
        }
    }
}
