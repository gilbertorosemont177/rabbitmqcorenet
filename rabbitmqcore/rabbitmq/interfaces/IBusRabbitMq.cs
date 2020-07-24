namespace rabbitmqcore.rabbitmq.interfaces
{
    internal   interface IBusRabbitMq
    {
        void sendCommand<T>(ICommand @command) where T : ICommand;
        void subscribe<T, TH>() where TH : IEventHandler<T>
            where T : IEvent;
    }
}
