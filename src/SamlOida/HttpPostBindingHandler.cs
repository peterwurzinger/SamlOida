using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SamlOida
{
    public class HttpPostBindingHandler : SamlBindingHandler
    {
        public HttpPostBindingHandler(SamlBindingOptions bindingOptions) : base(bindingOptions)
        {
        }

        protected internal override Task HandleAuthnRequestAsync(HttpResponse response, string authnRequest, string relayState)
        {
            //TODO: Maybe let this get rendered by a view enginge (e.g. Razor) to fit in the look-and-feel

            response.ContentType = "text/html";
            response.WriteAsync(
                $"<html><body><form action='{BindingOptions.IdentityProviderSignOnUrl}'>" +
                $"<input type='hidden' name='{SamlDefaults.SamlRequestQueryStringKey}' value='{authnRequest}'/>" +
                $"<input type='hidden' name='{SamlDefaults.RelayStateQueryStringKey}' value='{relayState}'/>" +
                $"<input type='submit' value='Login'/>" +
                $"</form></body></html>");
            return Task.FromResult(0);
        }
    }
}