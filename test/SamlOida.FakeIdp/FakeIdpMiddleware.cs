using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SamlOida.FakeIdp
{
    public class FakeIdpMiddleware
    {
        private readonly RequestDelegate _next;

        public const string LogOnPath = "/logon";

        public FakeIdpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext ctx)
        {
            if (ctx.Request.Path.HasValue && ctx.Request.Path == LogOnPath)
            {
                //Time to shine
                var samlRequest = ctx.Request.Query["SAMLRequest"];
                var document = Convert.FromBase64String(samlRequest).ToXmlDocument();

                switch (document.FirstChild.LocalName)
                {
                    case "AuthnRequest":
                        var parameters = new AuthnRequestParser().Parse(document);
                        var builder = new ResponseBuilder();
                        var doc = builder.Build(parameters);
                        var base64 = Convert.ToBase64String(doc.Deflate());
                        ctx.Response.Redirect($"{parameters.AssertionConsumerServiceURL}?SamlResponse={base64}&RelayState={ctx.Request.Query["RelayState"]}");
                        return Task.FromResult(0);
                    default: 
                        throw new NotImplementedException();
                }

            }
            return _next(ctx);
        }
    }
}
