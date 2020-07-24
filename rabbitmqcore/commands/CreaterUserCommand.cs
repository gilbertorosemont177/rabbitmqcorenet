using Newtonsoft.Json;
using rabbitmqcore.rabbitmq;
using rabbitmqcore.rabbitmq.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbitmqcore.commands
{
    [MessageNamespace("users")]
    public class CreaterUserCommand :ICommand
    {
       
        public string first_name;
        public string last_name;
        public string username;


        public Guid userId { get; set; }
        

        [JsonConstructor]
        public CreaterUserCommand(string firstname,string lastname,string username)
        {
            this.first_name = firstname;
            this.last_name = lastname;
            this.username = username;
            this.userId = userId;
        }

    }
}
