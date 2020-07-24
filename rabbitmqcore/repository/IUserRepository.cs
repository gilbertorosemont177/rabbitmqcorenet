using rabbitmqcore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbitmqcore.repository
{
    
    public interface IUserRepository:IRepository<Users> 
    {

        public string helloWorld();
    }
}
