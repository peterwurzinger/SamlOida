using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace SamlOida
{
    public class SamlHandler : RemoteAuthenticationHandler<SamlOptions>
    {
        protected override Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            //Authenticate user based on SamlResponse
            throw new NotImplementedException();
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            var result = AuthnRequestBuilder.Build(BuildRedirectUri(Options.CallbackPath));

            var base64 = WebUtility.UrlEncode(Convert.ToBase64String(result.Deflate()));

            //Redirect to IdP, SAML2 GET Profile
            Response.Redirect($"{AuthnRequestBuilder.SamlLogOn}?SAMLRequest={base64}&RelayState={WebUtility.UrlEncode(BuildRedirectUri(Options.CallbackPath))}");

            //Write XML out to Response
            //var bytes = Encoding.UTF8.GetBytes(result.InnerXml);
            //Response.ContentType = "text/xml";
            //Response.StatusCode = 200;
            //Response.Body.WriteAsync(bytes, 0, bytes.Length);

            return Task.FromResult(true);
        }
    }
}
