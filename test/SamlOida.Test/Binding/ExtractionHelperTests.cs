using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using SamlOida.Binding;
using SamlOida.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Xml;
using Xunit;

namespace SamlOida.Test.Binding
{
    public class ExtractionHelperTests
    {
        private readonly HttpContext _ctx;
        private readonly XmlDocument _message;

        public ExtractionHelperTests()
        {
            _ctx = new DefaultHttpContext();

            _message = new XmlDocument();

            var root = _message.CreateElement("RootNode");
            _message.AppendChild(root);
        }

        [Fact]
        public void ExtractRedirectShouldThrowExceptionIfNoPayloadFound()
        {
            Assert.Throws<SamlException>(() => ExtractionHelper.ExtractHttpRedirect(_ctx.Request.Query));
        }

        [Fact]
        public void ExtractRedirectShouldExtractMessage()
        {
            var query = new Dictionary<string, StringValues>
            {
                {SamlAuthenticationDefaults.SamlRequestKey, Convert.ToBase64String(_message.Deflate())}
            };
            _ctx.Request.Query = new QueryCollection(query);

            var result = ExtractionHelper.ExtractHttpRedirect(_ctx.Request.Query);

            Assert.Null(result.SignatureAlgorithm);
            Assert.Null(result.Signature);
            Assert.Null(result.RelayState);
            Assert.Equal(result.Message, _message);
        }

        [Fact]
        public void ExtractRedirectShouldExtractRelayState()
        {
            const string relayState = "TestRelayState";
            var query = new Dictionary<string, StringValues>
            {
                {SamlAuthenticationDefaults.SamlRequestKey, Convert.ToBase64String(_message.Deflate())},
                {SamlAuthenticationDefaults.RelayStateKey, relayState}
            };
            _ctx.Request.Query = new QueryCollection(query);

            var result = ExtractionHelper.ExtractHttpRedirect(_ctx.Request.Query);

            Assert.Null(result.SignatureAlgorithm);
            Assert.Null(result.Signature);
            Assert.Equal(relayState, result.RelayState);
            Assert.Equal(_message, result.Message);
        }

        [Fact]
        public void ExtractRedirectShouldExtractSignature()
        {
            var signature = Enumerable.Range(0, 10).Select(num => (byte)num).ToArray();

            var query = new Dictionary<string, StringValues>
            {
                {SamlAuthenticationDefaults.SamlRequestKey, Convert.ToBase64String(_message.Deflate())},
                {SamlAuthenticationDefaults.SignatureAlgorithmKey, SignedXml.XmlDsigRSASHA1Url},
                {SamlAuthenticationDefaults.SignatureKey, Convert.ToBase64String(signature)}
            };

            _ctx.Request.Query = new QueryCollection(query);

            var result = ExtractionHelper.ExtractHttpRedirect(_ctx.Request.Query);

            Assert.Equal(SignedXml.XmlDsigRSASHA1Url, result.SignatureAlgorithm);
            Assert.Equal(signature, result.Signature);
            Assert.Equal(_message, result.Message);
        }

        [Fact]
        public void ExtractPostShouldThrowExceptionIfNoPayloadFound()
        {
            _ctx.Request.ContentType = "application/x-www-form-urlencoded";

            Assert.Throws<SamlException>(() => ExtractionHelper.ExtractHttpPost(_ctx.Request.Form));
        }

        [Fact]
        public void ExtractPostShouldExtractMessage()
        {
            var form = new Dictionary<string, StringValues>
            {
                {SamlAuthenticationDefaults.SamlRequestKey, Convert.ToBase64String(_message.Deflate())}
            };
            _ctx.Request.Form = new FormCollection(form);

            var result = ExtractionHelper.ExtractHttpPost(_ctx.Request.Form);

            Assert.Null(result.SignatureAlgorithm);
            Assert.Null(result.Signature);
            Assert.Null(result.RelayState);
            Assert.Equal(_message, result.Message);
        }


        [Fact]
        public void ExtractPostShouldExtractRelayState()
        {
            const string relayState = "TestRelayState";

            var form = new Dictionary<string, StringValues>
            {
                {SamlAuthenticationDefaults.SamlRequestKey, Convert.ToBase64String(_message.Deflate())},
                {SamlAuthenticationDefaults.RelayStateKey, relayState}
            };
            _ctx.Request.Form = new FormCollection(form);

            var result = ExtractionHelper.ExtractHttpPost(_ctx.Request.Form);

            Assert.Null(result.SignatureAlgorithm);
            Assert.Null(result.Signature);
            Assert.Equal(relayState, result.RelayState);
            Assert.Equal(result.Message, _message);
        }
    }
}
