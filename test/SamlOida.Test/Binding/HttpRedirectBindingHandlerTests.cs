using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SamlOida.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Xml;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using SamlOida.MessageHandler;
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

        [Fact]
        public void ExtractShouldExtractMessage()
        {
            var ctx = new DefaultHttpContext();
            
            var query = new Dictionary<string, StringValues>();
            var messageBytes = _message.Deflate();
            query.Add(SamlAuthenticationDefaults.SamlRequestKey, Convert.ToBase64String(messageBytes));
            ctx.Request.Query = new QueryCollection(query);

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
            
            var query = new Dictionary<string, StringValues>();
            var messageBytes = _message.Deflate();
            const string relayState = "TestRelayState";
            query.Add(SamlAuthenticationDefaults.SamlRequestKey, Convert.ToBase64String(messageBytes));
            query.Add(SamlAuthenticationDefaults.RelayStateKey, relayState);
            ctx.Request.Query = new QueryCollection(query);

            var result = _target.ExtractMessage(ctx);

            Assert.Null(result.SignatureAlgorithm);
            Assert.Null(result.Signature);
            Assert.Equal(result.RelayState, relayState);
            Assert.Equal(result.Message, _message);
        }

        [Fact]
        public void ExtractShouldExtractSignature()
        {
            var ctx = new DefaultHttpContext();

            var query = new Dictionary<string, StringValues>();
            var messageBytes = _message.Deflate();
            query.Add(SamlAuthenticationDefaults.SamlRequestKey, Convert.ToBase64String(messageBytes));

            var signature = Enumerable.Range(0, 10).Select(num => (byte)num).ToArray();

            query.Add(SamlAuthenticationDefaults.SignatureAlgorithmKey, SignedXml.XmlDsigRSASHA1Url);
            query.Add(SamlAuthenticationDefaults.SignatureKey, Convert.ToBase64String(signature));

            ctx.Request.Query = new QueryCollection(query);

            var result = _target.ExtractMessage(ctx);

            Assert.Equal(SignedXml.XmlDsigRSASHA1Url, result.SignatureAlgorithm);
            Assert.Equal(signature, result.Signature);
            Assert.Equal(result.Message, _message);
        }

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