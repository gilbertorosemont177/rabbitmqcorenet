using System;
using Microsoft.AspNetCore.Mvc;
using rabbitmqcore.repository;
using rabbitmqcore.rabbitmq.interfaces;
using rabbitmqcore.commands;
using rabbitmqcore.rabbitmq;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using OpenTracing;

namespace rabbitmqcore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class WeatherForecastController : BaseController
    //ControllerBase 
    {
        public WeatherForecastController(IBusPublisher bus) : base(bus)
        {
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            Guid e = Guid.NewGuid();
            CreaterUserCommand d = new CreaterUserCommand(  "aurelie", " denis", "auregt2015@hotmail.com");
            await SendAsync(d.BindId(e=>e.userId),
                resourceId: d.userId, resource: "users");


            return "hello";
        }


    }
}

