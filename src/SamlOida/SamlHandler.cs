using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace SamlOida
{
    public class SamlHandler : RemoteAuthenticationHandler<SamlOptions>
    {
        protected override Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            //Authenticate based on SamlResponse
            throw new NotImplementedException();
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            //Redirect to IdP
            return Task.FromResult(true);
        }
    }
}
