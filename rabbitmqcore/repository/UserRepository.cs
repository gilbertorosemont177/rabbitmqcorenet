using rabbitmqcore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbitmqcore.repository
{

 
    public class UserRepository : Repository<Users>, IUserRepository
    {
        
        public UserRepository(LibraryContext db) : base(db)
        {
          
        }

        public string helloWorld()
        {
            return "Hello World From Canada";
        }
    }
    
}
