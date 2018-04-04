using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SamlOida.Binding;
using System;
using System.Xml;
using Xunit;

namespace SamlOida.Test.Binding
{
    public class HttpRedirectBindingHandlerTests
    {
        private readonly HttpRedirectBindingHandler _target;
        private readonly XmlDocument _message;
        private readonly SamlOptions _options;

        public HttpRedirectBindingHandlerTests()
        {
            _target = new HttpRedirectBindingHandler();

            _message = new XmlDocument();
            var el = _message.CreateElement("Message");
            _message.AppendChild(el);

            _options = new SamlOptions();
        }

        [Fact]
        public void ExtractShouldThrowExceptionIfNoPayloadFound()
        {
            var ctx = new DefaultHttpContext();

            Assert.Throws<SamlException>(() => _target.ExtractMessage(ctx));
        }

        //TODO: ExtractMessage-Tests

        [Fact]
        public void SendShouldApplyHttp302Redirection()
        {
            var ctx = new DefaultHttpContext();
            var targetUri = new Uri("http://test.com/saml-idp");

            _target.SendMessage(_options, ctx, _message, targetUri);

            Assert.Equal(302, ctx.Response.StatusCode);
            Assert.StartsWith(targetUri.ToString(), ctx.Response.Headers[HeaderNames.Location]);
        }

        [Fact]
        public void SendShouldPreserveRelayState()
        {
            var ctx = new DefaultHttpContext();
            var targetUri = new Uri("http://test.com/saml-idp");
            const string relayState = "TestRelayState";

            _target.SendMessage(_options, ctx, _message, targetUri, relayState);

            var queryString = QueryHelpers.ParseQuery(ctx.Response.Headers[HeaderNames.Location].ToString().Split('?')[1]);

            Assert.NotNull(queryString);
            Assert.True(queryString.ContainsKey(SamlAuthenticationDefaults.RelayStateKey));
            Assert.Equal(relayState, queryString[SamlAuthenticationDefaults.RelayStateKey]);

        }
    }
}