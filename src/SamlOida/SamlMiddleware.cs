using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SamlOida
{
    public class SamlMiddleware : AuthenticationMiddleware<SamlOptions>
    {

        public SamlMiddleware(RequestDelegate next, IOptions<SamlOptions> options, ILoggerFactory loggerFactory, UrlEncoder encoder) : base(next, options, loggerFactory, encoder)
        {
        }
        
        protected override AuthenticationHandler<SamlOptions> CreateHandler()
        {
            return new SamlHandler();
        }
    }

}
