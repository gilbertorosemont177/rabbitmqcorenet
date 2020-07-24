using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using rabbitmqcore.rabbitmq.interfaces;
using RawRabbit;
using RawRabbit.Common;


using OpenTracing;
using OpenTracing.Tag;


namespace rabbitmqcore.rabbitmq
{
    public class BusSubscriber : IBusSubscriber
    {
        private readonly ILogger _logger;
        private readonly IBusClient _busClient;
        private readonly IServiceProvider _serviceProvider;
      
        private readonly int _retries;
        private readonly int _retryInterval;
        private readonly ITracer _tracer;
        public BusSubscriber(IApplicationBuilder app)
        {
            _logger = app.ApplicationServices.GetService<ILogger<BusSubscriber>>();
            _serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();
            _busClient = _serviceProvider.GetService<IBusClient>();
      
            var options = _serviceProvider.GetService<RabbitMqOptions>();
            _retries = options.Retries >= 0 ? options.Retries : 3;
            _retryInterval = options.RetryInterval > 0 ? options.RetryInterval : 2;
        }

        public IBusSubscriber SubscribeCommand<TCommand>(string @namespace = null, string queueName = null)
           where TCommand : ICommand
        {
            _busClient.SubscribeAsync<TCommand, CorrelationContext>(async (command, correlationContext) =>
            {
                var commandHandler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
                ICorrelationContext c = correlationContext;
                ICommand _c = command;
                return await TryHandleAsync(commandHandler.HandleAsync(command, correlationContext));
            });

            return this;
        }

        private async Task<Acknowledgement> TryHandleAsync(Task task)
        {
            return new Ack();
        }

      

    }
}