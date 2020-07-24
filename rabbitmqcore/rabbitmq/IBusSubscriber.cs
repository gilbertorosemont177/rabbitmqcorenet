using rabbitmqcore.rabbitmq.interfaces;

namespace rabbitmqcore.rabbitmq
{
    public interface IBusSubscriber
    {


        IBusSubscriber SubscribeCommand<TCommand>(string @namespace = null, string queueName = null)
            where TCommand : ICommand;
    }
}