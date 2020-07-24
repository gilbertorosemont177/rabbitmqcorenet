using Autofac;
using Microsoft.Extensions.Configuration;
using rabbitmqcore.common;
using rabbitmqcore.rabbitmq.interfaces;
using RawRabbit.Configuration;
using System.Reflection;
using RawRabbit;
using System;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.Instantiation;
using RawRabbit.Pipe;
using RawRabbit.Pipe.Middleware;
using RawRabbit.Enrichers.Attributes.Middleware;
using System.Threading.Tasks;
using System.Threading;
using RawRabbit.Enrichers.MessageContext;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace rabbitmqcore.rabbitmq
{
    public static class Extensions
    {

        public static IBusSubscriber UseRabbitMq(this IApplicationBuilder app)
           => new BusSubscriber(app);
        public static void AddRabbitMq(this ContainerBuilder builder) {

            builder.Register(ctx =>
            {
               
                var configuration = ctx.Resolve<IConfiguration>();
              
                var rawrabbitoption = configuration.GetOptionFromJsonSettings<RabbitMqOptions>("rabbitmq");

                return rawrabbitoption;
            }).SingleInstance();

            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var rawrabbitoption = configuration.GetOptionFromJsonSettings<RawRabbitConfiguration>("rabbitMq");

                return rawrabbitoption;
            }).SingleInstance();

            var assembly = Assembly.GetCallingAssembly();
       
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(ICommandHandler<>))
                .InstancePerDependency();
            builder.RegisterType<BusPublisher>().As<IBusPublisher>()
                .InstancePerDependency();
            ConfigureBus(builder);


        }

        private static void ConfigureBus(ContainerBuilder builder)
        {
            builder.Register<IInstanceFactory>(context =>
            {
                var options = context.Resolve<RabbitMqOptions>();
                var configuration = context.Resolve<RawRabbitConfiguration>();
                var namingConventions = new CustomNamingConventions("users");
             
                return RawRabbitFactory.CreateInstanceFactory(new RawRabbitOptions
                {
                    DependencyInjection = ioc =>
                    {
                        ioc.AddSingleton(options);
                        ioc.AddSingleton(configuration);
                        ioc.AddSingleton<INamingConventions>(namingConventions);

                    },
                    Plugins = p => p
                        .UseAttributeRouting()
                        .UseRetryLater()
                        .UpdateRetryInfo()
                     
                       .UseMessageContext<CorrelationContext>()
                       .UseContextForwarding()
               
                });
            }).SingleInstance();
            builder.Register(context => context.Resolve<IInstanceFactory>().Create());
            
        }
        
        private class RetryStagedMiddleware : StagedMiddleware
        {
            public override string StageMarker => RawRabbit.Pipe.StageMarker.MessageDeserialized;
          
            private readonly Serilog.ILogger _logger = Log.ForContext<RetryStagedMiddleware>();

            public override async Task InvokeAsync(IPipeContext context, CancellationToken ct)
            {
                var msgType = context.GetMessageType();
                _logger.Information("Message of type {messageType} just published", msgType.Name);
                await Next.InvokeAsync(context, ct);
            }
        }

        
        public class CustomNamingConventions : NamingConventions
        {
            public CustomNamingConventions(string defaultNamespace)
            {
                var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
                ExchangeNamingConvention = type => GetNamespace(type, defaultNamespace).ToLowerInvariant();
                RoutingKeyConvention = type =>
                    $"{GetRoutingKeyNamespace(type, defaultNamespace)}{type.Name.Underscore()}".ToLowerInvariant();
                QueueNamingConvention = type => GetQueueName(assemblyName, type, defaultNamespace);
                ErrorExchangeNamingConvention = () => $"{defaultNamespace}.error";
                RetryLaterExchangeConvention = span => $"{defaultNamespace}.retry";
                RetryLaterQueueNameConvetion = (exchange, span) =>
                    $"{defaultNamespace}.retry_for_{exchange.Replace(".", "_")}_in_{span.TotalMilliseconds}_ms".ToLowerInvariant();
            }

            private static string GetRoutingKeyNamespace(Type type, string defaultNamespace)
            {
                var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;

                return string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";
            }

            private static string GetNamespace(Type type, string defaultNamespace)
            {
                var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;

                return string.IsNullOrWhiteSpace(@namespace) ? type.Name.Underscore() : $"{@namespace}";
            }

            public static string GetQueueName(string assemblyName, Type type, string defaultNamespace)
            {
                var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;
                var separatedNamespace = string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";

                return $"{assemblyName}/{separatedNamespace}{type.Name.Underscore()}".ToLowerInvariant();
            }

            
        }

        private static IClientBuilder UpdateRetryInfo(this IClientBuilder clientBuilder)
        {
            clientBuilder.Register(c => c.Use<RetryStagedMiddleware>());

            return clientBuilder;
        }
        public static string GetQueueName<T>(string _namespace, string queueName)
        {
           
            var separatedNamespace = string.IsNullOrWhiteSpace(_namespace) ? string.Empty : $"{_namespace}.";

            return $"{_namespace}/{separatedNamespace}{queueName.Underscore()}".ToLowerInvariant();
        }
    }
}
