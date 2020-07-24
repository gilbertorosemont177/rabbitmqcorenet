using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rabbitmqcore.Models;
using rabbitmqcore.repository;
using Autofac;
using rabbitmqcore.rabbitmq;
using Autofac.Extensions.DependencyInjection;
using System;
using System.Reflection;
using rabbitmqcore.commands;

namespace rabbitmqcore
{
    public class Startup
    {
      
        public IConfiguration Configuration { get; }
        public IContainer Container { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
           services.AddDbContext<LibraryContext>(ServiceLifetime.Transient);
           services.AddControllers();
           services.AddTransient<IUserRepository, UserRepository>();
        
            ContainerBuilder builder = new ContainerBuilder();
           
            builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly())
               .AsImplementedInterfaces();
            builder.Populate(services);
            builder.AddRabbitMq();
            Container = builder.Build();

            return new AutofacServiceProvider(Container);
           
        }
      
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           // app.UseSession();
        
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRabbitMq().SubscribeCommand<CreaterUserCommand>();
                
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
          
        }
    }
}

