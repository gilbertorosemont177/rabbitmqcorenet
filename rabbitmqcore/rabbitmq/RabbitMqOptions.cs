using RawRabbit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbitmqcore.rabbitmq
{
    public class RabbitMqOptions: RawRabbitConfiguration
    {
        public int Retries { get; set; }
        public int RetryInterval { get; set; }
        public string Namespace { get;  set; }
    }
}
