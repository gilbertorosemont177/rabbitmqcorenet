using System.Reflection;
using System.Threading.Tasks;
using rabbitmqcore.common;
using RawRabbit;
using RawRabbit.Enrichers.MessageContext;


namespace rabbitmqcore.rabbitmq.interfaces
{
    public class BusPublisher : IBusPublisher
    {

        private readonly IBusClient _busClient;
        private readonly string _defaultNamespace;

        private string GetNameSpace<T>(T message) {

               var @namespace = message.GetType().GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? _defaultNamespace;
                @namespace = string.IsNullOrWhiteSpace(@namespace)? string.Empty : $"{@namespace}";
                return @namespace;
        }





        public BusPublisher(IBusClient busClient,RabbitMqOptions options)
        {
            _busClient = busClient;
            _defaultNamespace = options.Namespace;
        }

        public async Task SendAsync<TCommand>(TCommand command, ICorrelationContext context) where TCommand : ICommand
           => await _busClient.PublishAsync(command, ctx => ctx.UseMessageContext(context).UsePublishConfiguration(p
                   =>
               {
                   p.WithRoutingKey(GetRoutingKey(command));
                   p.OnExchange(GetNameSpace(command));

               }
           ));




        public async Task PublishAsync<TEvent>(TEvent @event, ICorrelationContext context) where TEvent : IEvent
           => await _busClient.PublishAsync(@event, ctx => ctx.UseMessageContext(context));
        
        private string GetRoutingKey<T>(T message)
        {
           var @namespace=GetNameSpace(message);

            return $"{@namespace}.{typeof(T).Name.Underscore()}".ToLowerInvariant();


        }

        
    }


}
