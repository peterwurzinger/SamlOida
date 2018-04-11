using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SamlOida.Binding;
using System.Xml;
using Xunit;

namespace SamlOida.Test.Binding
{
    public class HttpRedirectBindingHandlerTests
    {
        private readonly HttpRedirectBindingHandler _target;
        private readonly XmlDocument _message;
        private readonly SamlOptions _options;
        private readonly HttpContext _ctx;
        private readonly string _uri;

        public HttpRedirectBindingHandlerTests()
        {
            _target = new HttpRedirectBindingHandler();

            _message = new XmlDocument();
            var root = _message.CreateElement("Message");
            _message.AppendChild(root);

            _options = new SamlOptions();
            _ctx = new DefaultHttpContext();
            _uri = "http://test.com:8080/saml-idp";
        }

        [Fact]
        public void BindShouldApplyHttp302Redirection()
        {
            _target.BindMessage(_message, _ctx, _uri, SamlAuthenticationDefaults.SamlRequestKey, _options);

            Assert.Equal(302, _ctx.Response.StatusCode);
            Assert.StartsWith(_uri, _ctx.Response.Headers[HeaderNames.Location]);
        }

        [Fact]
        public void BindShouldPreserveRelayState()
        {
            const string relayState = "TestRelayState";

            _target.BindMessage(_message, _ctx, _uri, SamlAuthenticationDefaults.SamlRequestKey, _options, relayState);

            var queryString = QueryHelpers.ParseQuery(_ctx.Response.Headers[HeaderNames.Location].ToString().Split('?')[1]);

            Assert.NotNull(queryString);
            Assert.True(queryString.ContainsKey(SamlAuthenticationDefaults.RelayStateKey));
            Assert.Equal(relayState, queryString[SamlAuthenticationDefaults.RelayStateKey]);

        }
    }
}