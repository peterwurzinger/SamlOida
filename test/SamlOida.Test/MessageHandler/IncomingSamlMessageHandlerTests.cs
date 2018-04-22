using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using SamlOida.Binding;
using SamlOida.Test.MessageHandler.Parser;
using SamlOida.Test.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SamlOida.Test.MessageHandler
{
    public class IncomingSamlMessageHandlerTests
    {

        private readonly FakeIncomingSamlMessageHandler _target;
        private readonly FakeSamlMessageParserBase _parser;
        private readonly SamlOptions _options;
        private readonly DefaultHttpContext _context;

        public IncomingSamlMessageHandlerTests()
        {
            _parser = new FakeSamlMessageParserBase { ParseResult = new FakeSamlMessage() };
            _target = new FakeIncomingSamlMessageHandler(_parser);
            _options = new SamlOptions
            {
                AcceptSignedMessagesOnly = false
            };
            _context = new DefaultHttpContext();
            _context.Request.Method = "GET";

            var dict = new Dictionary<string, StringValues>
            {
                {SamlAuthenticationDefaults.SamlRequestKey, Convert.ToBase64String(Encoding.UTF8.GetBytes("<Payload/>"))}
            };

            _context.Request.Query = new QueryCollection(dict);
        }

        [Fact]
        public void ConstructorShouldThrowExceptionIfParserIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new FakeIncomingSamlMessageHandler(null));
        }

        [Fact]
        public void HandleShouldThrowIfHttpContextIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _target.Handle(_options, null));
        }

        [Fact]
        public void HandleShouldParseMessage()
        {
            _target.Handle(_options, _context);

            Assert.True(_parser.ParseCalled);
        }

        [Fact]
        public void HandleShouldValidateMessage()
        {
            _target.Handle(_options, _context);

            Assert.True(_target.ValidateCalled);
        }

        [Fact]
        public void HandleShouldCallHandleInternal()
        {
            _target.Handle(_options, _context);

            Assert.True(_target.HandleInternalCalled);
        }

        [Fact]
        public void ValidateShouldThrowExceptionIfMessageIsUnsignedButNeedsToBe()
        {
            var parsingResult = new FakeSamlMessage
            {
                HasValidSignature = false
            };
            var extractionResult = new ExtractionResult
            {
                HasValidSignature = false
            };
            _options.AcceptSignedMessagesOnly = true;

            Assert.Throws<SamlException>(() => _target.Validate(_options, extractionResult, parsingResult));
        }

        [Fact]
        public void ValidateShouldNotThrowExceptionIfMessageIsSigned()
        {
            var parsingResult = new FakeSamlMessage
            {
                HasValidSignature = true
            };
            var extractionResult = new ExtractionResult
            {
                HasValidSignature = false
            };
            _options.AcceptSignedMessagesOnly = true;

            _target.Validate(_options, extractionResult, parsingResult);
        }

        [Fact]
        public void ValidateShouldNotThrowExceptionIExtractionResultIsSigned()
        {
            var parsingResult = new FakeSamlMessage
            {
                HasValidSignature = false
            };
            var extractionResult = new ExtractionResult
            {
                HasValidSignature = true
            };
            _options.AcceptSignedMessagesOnly = true;

            _target.Validate(_options, extractionResult, parsingResult);
        }
    }
}
