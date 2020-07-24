using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbitmqcore.rabbitmq.interfaces
{
   
        public interface ICommandHandler<in TCommand> where TCommand : ICommand
        {
            Task HandleAsync(TCommand command, CorrelationContext correlationContext);
        }
    
}
