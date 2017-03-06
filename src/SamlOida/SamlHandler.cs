using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace SamlOida
{
    public class SamlHandler : RemoteAuthenticationHandler<SamlOptions>
    {
        protected override Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
