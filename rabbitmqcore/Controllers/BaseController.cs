using Microsoft.AspNetCore.Mvc;
using rabbitmqcore.rabbitmq;
using rabbitmqcore.rabbitmq.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenTracing;
using System.Threading.Tasks;

namespace rabbitmqcore.Controllers
{
    public abstract class BaseController:  ControllerBase
    {


        private readonly string AcceptLanguageHeader = "accept-language";
        private readonly string OperationHeader = "X-Operation";
        private readonly string ResourceHeader = "X-Resource";
        private readonly string DefaultCulture = "en-us";
        private readonly string PageLink = "page";


     


        private readonly IBusPublisher _bus;

        public BaseController(IBusPublisher bus)
        {

        
            _bus = bus;
           
        }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };



        protected async Task<IActionResult> SendAsync<T>(T command,
    Guid? resourceId = null, string resource = "") where T : ICommand
        {
            var context = GetContext<T>(resourceId, resource);
            await _bus.SendAsync(command,context);

            return Accepted(context);

        }

        protected IActionResult Accepted(ICorrelationContext context)
        {
            Response.Headers.Add(OperationHeader, $"operations/{context.Id}");
            if (!string.IsNullOrWhiteSpace(context.Resource))
            {
                Response.Headers.Add(ResourceHeader, context.Resource);
            }

            return base.Accepted();
        }

        protected ICorrelationContext GetContext<T>(Guid? resourceId = null, string resource = "") where T : ICommand
        {
            if (!string.IsNullOrWhiteSpace(resource))
            {
                resource = $"{resource}/{resourceId}";
            }

            return CorrelationContext.Create<T>(Guid.NewGuid(), Guid.NewGuid(), resourceId ?? Guid.Empty,
               HttpContext.TraceIdentifier, HttpContext.Connection.Id,"",
               Request.Path.ToString(), Culture, resource);
        }

        protected bool IsAdmin
            => User.IsInRole("admin");

        protected Guid UserId
            => string.IsNullOrWhiteSpace(User?.Identity?.Name) ?
                Guid.Empty :
                Guid.Parse(User.Identity.Name);

        protected string Culture
            => Request.Headers.ContainsKey(AcceptLanguageHeader) ?
                    Request.Headers[AcceptLanguageHeader].First().ToLowerInvariant() :
                    DefaultCulture;




        private static string FormatLink(string path, string rel)
            => string.IsNullOrWhiteSpace(path) ? string.Empty : $"<{path}>; rel=\"{rel}\",";
    }

}

