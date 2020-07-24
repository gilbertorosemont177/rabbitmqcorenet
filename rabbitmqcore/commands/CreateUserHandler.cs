using rabbitmqcore.rabbitmq.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rabbitmqcore.repository;
using rabbitmqcore.Models;
using rabbitmqcore.rabbitmq;

namespace rabbitmqcore.commands
{
    public class CreateUserHandler:  ICommandHandler<CreaterUserCommand>
    {
        private readonly IUserRepository repo;
        private readonly IBusPublisher _busPublisher;


        public CreateUserHandler(
            IUserRepository productsRepository,
            IBusPublisher busPublisher)
        {
            repo = productsRepository;
            _busPublisher = busPublisher;
        }

        public  async Task HandleAsync(CreaterUserCommand command,CorrelationContext correlationContext)
        {
            var usercom = new Users { FirstName = command.first_name, LastName = command.last_name, Username = command.username };
            

            repo.Save(usercom);
          
        }

       
    }
}
