using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SamlOida
{
    public abstract class SamlBindingHandler
    {
        protected readonly SamlBindingOptions BindingOptions;

        protected SamlBindingHandler(SamlBindingOptions bindingOptions)
        {
            BindingOptions = bindingOptions;
        }

        protected internal abstract Task HandleAuthnRequestAsync(HttpResponse response, string authnRequest, string relayState);
    }
}
