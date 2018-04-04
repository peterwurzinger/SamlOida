using Microsoft.AspNetCore.WebUtilities;
using SamlOida.Binding;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml;
using Xunit;

namespace SamlOida.Test.Binding
{
    public class EncodingHelperTest
    {
        private readonly SamlOptions _options;
        private readonly XmlDocument _message;

        public EncodingHelperTest()
        {
            _options = new SamlOptions();

            _message = new XmlDocument();
            var el = _message.CreateElement("Message");
            _message.AppendChild(el);
        }

        [Fact]
        public void EncodeShouldThrowExceptionOnUnknownEncoding()
        {
            Assert.Throws<NotImplementedException>(() =>
                EncodingHelper.EncodeMessage(_options, "Unknown Encoding", new XmlDocument(), "SAMLRequest", null));
        }

        [Fact]
        public void EncodeShouldPreserveRelayState()
        {
            const string relayState = "TestRelayState";

            var querystring = EncodingHelper.EncodeMessage(_options, SamlAuthenticationDefaults.DeflateEncoding, _message, SamlAuthenticationDefaults.SamlRequestKey,
                relayState);

            Assert.True(querystring.HasValue);
            var query = QueryHelpers.ParseQuery(querystring.ToString());

            Assert.True(query.TryGetValue(SamlAuthenticationDefaults.RelayStateKey, out var actualRelayState));
            Assert.Equal(relayState, WebUtility.UrlDecode(actualRelayState));
        }

        [Fact]
        public void EncodeShouldAppendDeflatedMessageAsBase64()
        {
            var querystring = EncodingHelper.EncodeMessage(_options, SamlAuthenticationDefaults.DeflateEncoding, _message, SamlAuthenticationDefaults.SamlRequestKey,
                null);

            Assert.True(querystring.HasValue);
            var query = QueryHelpers.ParseQuery(querystring.ToString());


            Assert.True(query.TryGetValue(SamlAuthenticationDefaults.SamlRequestKey, out var actualSamlRequest));
            var messageBytes = Convert.FromBase64String(WebUtility.UrlDecode(actualSamlRequest));

            var actualMessage = new XmlDocument();
            using (var memoryStream = new MemoryStream(messageBytes))
            {
                using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
                {
                    actualMessage.Load(deflateStream);
                }
            }

            Assert.Equal(_message, actualMessage);
        }

        [Fact]
        public void EncodeShouldNotAppendRelayStateIfNull()
        {
            var querystring = EncodingHelper.EncodeMessage(_options, SamlAuthenticationDefaults.DeflateEncoding, _message, SamlAuthenticationDefaults.SamlRequestKey,
                null);

            Assert.True(querystring.HasValue);
            var query = QueryHelpers.ParseQuery(querystring.ToString());

            Assert.False(query.TryGetValue(SamlAuthenticationDefaults.RelayStateKey, out var _));
        }

        //TODO: Test Signature
    }
}

