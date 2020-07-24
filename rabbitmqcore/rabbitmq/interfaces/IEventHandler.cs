using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbitmqcore.rabbitmq.interfaces
{
    public interface IEventHandler<in T> : IEventHandler where T : IEvent 
    {
        Task<string> handle(T @event);
    }

    public interface IEventHandler
    {

    }
}
