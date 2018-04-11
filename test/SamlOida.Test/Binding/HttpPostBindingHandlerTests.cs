using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using SamlOida.Binding;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Xml;
using Xunit;

namespace SamlOida.Test.Binding
{
    public class HttpPostBindingHandlerTests
    {
        private readonly HttpPostBindingHandler _target;
        private readonly XmlDocument _message;
        private readonly SamlOptions _options;
        private readonly HttpContext _ctx;
        private readonly string _uri;

        public HttpPostBindingHandlerTests()
        {
            _target = new HttpPostBindingHandler();
            _ctx = new DefaultHttpContext();

            _ctx.Features.Set<IHttpResponseFeature>(new HttpResponseFeature
            {
                Body = new MemoryStream()
            });

            _uri = "http://test.com/saml-idp";

            _message = new XmlDocument();
            var el = _message.CreateElement("Message");
            _message.AppendChild(el);

            _options = new SamlOptions();
        }


        [Fact]
        public void BindShouldApplyPostForm()
        {

            _target.BindMessage(_message, _ctx, _uri, SamlAuthenticationDefaults.SamlRequestKey, _options);

            _ctx.Response.Body.Position = 0;

            string htmlPage;
            using (var reader = new StreamReader(_ctx.Response.Body))
            {
                htmlPage = reader.ReadToEnd();
            }

            Assert.Equal(200, _ctx.Response.StatusCode);
            Assert.NotNull(htmlPage);
            Assert.NotEqual(string.Empty, htmlPage);
            Assert.Equal("text/html", _ctx.Response.ContentType);
            Assert.Contains($"<html><body><form action='{_uri}' method='POST'>", htmlPage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void BindShouldPreserveRelayState()
        {
            //https://stackoverflow.com/questions/46433309/response-on-created-context-keeps-giving-me-nullstream
            var ctx = new DefaultHttpContext();
            ctx.Features.Set<IHttpResponseFeature>(new HttpResponseFeature
            {
                Body = new MemoryStream()
            });

            var targetUri = "http://test.com/saml-idp";
            const string relayState = "TestRelayState";

            _target.BindMessage(_message, ctx, targetUri, SamlAuthenticationDefaults.SamlRequestKey, _options, relayState);

            ctx.Response.Body.Position = 0;
            string htmlPage;
            using (var reader = new StreamReader(ctx.Response.Body))
            {
                htmlPage = reader.ReadToEnd();
            }

            Assert.Equal(200, ctx.Response.StatusCode);
            Assert.NotNull(htmlPage);
            Assert.Contains($"<input type='hidden' name='{SamlAuthenticationDefaults.RelayStateKey}' value='{HtmlEncoder.Default.Encode(relayState)}'/>", htmlPage, StringComparison.OrdinalIgnoreCase);

        }
    }
}
