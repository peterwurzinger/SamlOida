using Microsoft.AspNetCore.Http;
using SamlOida.Binding;
using SamlOida.Test.Binding;
using SamlOida.Test.MessageHandler.MessageFactory;
using SamlOida.Test.Model;
using System;
using Xunit;

namespace SamlOida.Test.MessageHandler
{
    public class OutgoingSamlMessageHandlerTests
    {
        private readonly FakeOutgoingSamlMessageHandler _target;
        private readonly FakeSamlMessageFactory _factory;
        private readonly FakeBinding _binding;
        private readonly SamlOptions _options;

        public OutgoingSamlMessageHandlerTests()
        {
            _factory = new FakeSamlMessageFactory();
            _binding = new FakeBinding();
            _target = new FakeOutgoingSamlMessageHandler(_factory, new []{_binding});
            _options = new SamlOptions();
        }

        [Fact]
        public void ConstructorShouldThrowExceptionIfBindingIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new FakeOutgoingSamlMessageHandler(_factory, null));
        }

        [Fact]
        public void ConstructorShouldThrowExceptionIfFactoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new FakeOutgoingSamlMessageHandler(null, new []{_binding}));
        }

        [Fact]
        public void HandleShouldCallMessageFactory()
        {
            _target.Handle(_options, new DefaultHttpContext(), new FakeSamlMessage(), "target", SamlBindingBehavior.HttpRedirect);

            Assert.True(_factory.CreateMessageCalled);
        }

        [Fact]
        public void HandleShouldBindMessage()
        {
            _target.Handle(_options, new DefaultHttpContext(), new FakeSamlMessage(), "target", SamlBindingBehavior.HttpRedirect);

            Assert.True(_binding.BindMessageCalled);
        }
    }
}
