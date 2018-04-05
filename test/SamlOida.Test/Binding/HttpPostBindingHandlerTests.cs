using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using SamlOida.Binding;
using SamlOida.MessageHandler;
using System;
using System.Collections.Generic;
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

        public HttpPostBindingHandlerTests()
        {
            _target = new HttpPostBindingHandler();

            _message = new XmlDocument();
            var el = _message.CreateElement("Message");
            _message.AppendChild(el);

            _options = new SamlOptions();
        }

        [Fact]
        public void ExtractShouldThrowExceptionIfNoPayloadFound()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.ContentType = "application/x-www-form-urlencoded";

            Assert.Throws<SamlException>(() => _target.ExtractMessage(ctx));
        }

        [Fact]
        public void ExtractShouldExtractMessage()
        {
            var ctx = new DefaultHttpContext();


            var form = new Dictionary<string, StringValues>();
            var messageBytes = _message.Deflate();
            form.Add(SamlAuthenticationDefaults.SamlRequestKey, Convert.ToBase64String(messageBytes));
            ctx.Request.Form = new FormCollection(form);

            var result = _target.ExtractMessage(ctx);

            Assert.Null(result.SignatureAlgorithm);
            Assert.Null(result.Signature);
            Assert.Null(result.RelayState);
            Assert.Equal(result.Message, _message);
        }

        [Fact]
        public void ExtractShouldExtractRelayState()
        {
            var ctx = new DefaultHttpContext();

            var form = new Dictionary<string, StringValues>();
            var messageBytes = _message.Deflate();
            const string relayState = "TestRelayState";
            form.Add(SamlAuthenticationDefaults.SamlRequestKey, Convert.ToBase64String(messageBytes));
            form.Add(SamlAuthenticationDefaults.RelayStateKey, relayState);
            ctx.Request.Form = new FormCollection(form);

            var result = _target.ExtractMessage(ctx);

            Assert.Null(result.SignatureAlgorithm);
            Assert.Null(result.Signature);
            Assert.Equal(result.RelayState, relayState);
            Assert.Equal(result.Message, _message);
        }

        [Fact]
        public void SendShouldApplyPostForm()
        {
            //https://stackoverflow.com/questions/46433309/response-on-created-context-keeps-giving-me-nullstream
            var ctx = new DefaultHttpContext();
            ctx.Features.Set<IHttpResponseFeature>(new HttpResponseFeature
            {
                Body = new MemoryStream()
            });

            var targetUri = new Uri("http://test.com/saml-idp");

            _target.SendMessage(_options, ctx, _message, targetUri);

            ctx.Response.Body.Position = 0;
            string htmlPage;
            using (var reader = new StreamReader(ctx.Response.Body))
            {
                htmlPage = reader.ReadToEnd();
            }

            Assert.Equal(200, ctx.Response.StatusCode);
            Assert.NotNull(htmlPage);
            Assert.NotEqual(string.Empty, htmlPage);
            Assert.Equal("text/html", ctx.Response.ContentType);
            Assert.Contains($"<html><body><form action='{targetUri.AbsoluteUri}' method='POST'>", htmlPage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SendShouldPreserveRelayState()
        {
            //https://stackoverflow.com/questions/46433309/response-on-created-context-keeps-giving-me-nullstream
            var ctx = new DefaultHttpContext();
            ctx.Features.Set<IHttpResponseFeature>(new HttpResponseFeature
            {
                Body = new MemoryStream()
            });

            var targetUri = new Uri("http://test.com/saml-idp");
            const string relayState = "TestRelayState";

            _target.SendMessage(_options, ctx, _message, targetUri, relayState);

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
