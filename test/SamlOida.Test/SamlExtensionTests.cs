using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using SamlOida.MessageHandler;
using System.Linq;
using System.Reflection;
using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using Xunit;

namespace SamlOida.Test
{
    public class SamlExtensionTests
    {
        private readonly ServiceCollection _services;
        private readonly AuthenticationBuilder _authBuilder;

        public SamlExtensionTests()
        {
            _services = new ServiceCollection();
            _authBuilder = new AuthenticationBuilder(_services);
        }

        [Fact]
        public void AddShouldRegisterMessageHandlers()
        {
            _authBuilder.AddSaml();

            var messageHandlerTypes = typeof(SamlHandler).Assembly.GetTypes().Where(t =>
                t.Namespace == typeof(AuthnRequestHandler).Namespace && t.IsPublic && !t.IsAbstract && t.BaseType.IsGenericType 
                    && (t.BaseType.GetGenericTypeDefinition() == typeof(OutgoingSamlMessageHandler<>) || t.BaseType.GetGenericTypeDefinition() == typeof(IncomingSamlMessageHandler<,>)))
                .Select(type => ServiceDescriptor.Scoped(type, type))
                .ToList();

            Assert.NotEmpty(messageHandlerTypes);
            Assert.True(messageHandlerTypes.All(handlerDescriptor => _services.SingleOrDefault(service => service.ImplementationType == handlerDescriptor.ImplementationType
                                                                                                            && service.ServiceType == handlerDescriptor.ServiceType
                                                                                                          && service.Lifetime == handlerDescriptor.Lifetime) != null));
        }

        [Fact]
        public void AddShouldregisterBindings()
        {
            _authBuilder.AddSaml();

            var bindings = typeof(SamlHandler).Assembly.GetTypes().Where(t =>
                    t.Namespace == typeof(ISamlBindingStrategy).Namespace && t.IsPublic && !t.IsAbstract && t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ISamlBindingStrategy)))
                .Select(type => ServiceDescriptor.Singleton(typeof(ISamlBindingStrategy), type))
                .ToList();

            Assert.NotEmpty(bindings);
            Assert.True(bindings.All(bindingDescriptor => _services.SingleOrDefault(service => service.ImplementationType == bindingDescriptor.ImplementationType
                                                                                                          && service.ServiceType == bindingDescriptor.ServiceType
                                                                                                          && service.Lifetime == bindingDescriptor.Lifetime) != null));
        }

        [Fact]
        public void AddShouldRegisterMessageFactories()
        {
            _authBuilder.AddSaml();

            var factories = typeof(SamlHandler).Assembly.GetTypes().Where(t =>
                    t.Namespace == typeof(ISamlMessageFactory<>).Namespace && t.IsPublic && !t.IsAbstract && 
                    t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISamlMessageFactory<>)))
                .Select(type => ServiceDescriptor.Singleton(type, type))
                .ToList();

            Assert.NotEmpty(factories);
            Assert.True(factories.All(factoryDescriptor => _services.SingleOrDefault(service => service.ImplementationType == factoryDescriptor.ImplementationType
                                                                                               && service.ServiceType == factoryDescriptor.ServiceType
                                                                                               && service.Lifetime == factoryDescriptor.Lifetime) != null));
        }
    }
}
