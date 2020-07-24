using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rabbitmqcore.rabbitmq.interfaces;
namespace rabbitmqcore.events
{
    public class UserCreatedEvent : IEvent
    {
        public UserCreatedEvent(string name, string password) { 
        

        }

        
    }
}
