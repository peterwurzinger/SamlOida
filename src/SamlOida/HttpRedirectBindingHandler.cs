using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SamlOida
{
    public class HttpRedirectBindingHandler : SamlBindingHandler
    {
        public HttpRedirectBindingHandler(SamlBindingOptions bindingOptions) : base(bindingOptions)
        {
        }

        protected internal override Task HandleAuthnRequestAsync(HttpResponse response, string authnRequest, string relayState)
        {
            response.Redirect($"{BindingOptions.IdentityProviderSignOnUrl}?{SamlDefaults.SamlRequestQueryStringKey}={authnRequest}&{SamlDefaults.RelayStateQueryStringKey}={WebUtility.UrlEncode(relayState)}");
            return Task.FromResult(0);
        }
    }
}