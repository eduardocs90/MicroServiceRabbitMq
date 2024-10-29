using Domain.Repositories;
using InfraData.Context;
using InfraData.Repositories;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using UserService.Service;
/*using RabbitMQ.Client;*/
/*using Service.RabbitMq;*/

namespace CrossCutting.Configuration
{
    public static class DependencyInjection
    {        //Injecão de dependencias entre as interfaceRepositorios de Domain para os repositórios de InfraData
        public static IServiceCollection AddInfrastruture(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ContextDb>(options => options.UseNpgsql(configuration.GetSection("ConnectionStrings:DefaultConnection").Value));
            services.AddScoped<IUserRepository, UserRepository>();
           

            /*  var factory = new ConnectionFactory
              {
                  HostName = configuration["RabbitMQ:HostName"],
                  UserName = configuration["RabbitMQ:UserName"],
                  Password = configuration["RabbitMQ:Password"]
              };

              var connection = factory.CreateConnection();
              services.AddSingleton<IConnection>(connection);
              services.AddSingleton<RabbitMqService>();
              services.AddSingleton<RabbitMqEmailConsumer>();
  */

            return services;
        }
        // Injeção de dependencias entre as interfacesServices e os Services do Projeto Services
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService.Service.UserService>();
            services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();
            services.AddSingleton<IEmailService, EmailService>();
            /*
              services.AddAutoMapper(typeof(DtoToDomain));
              services.AddAutoMapper(typeof(DomainToDto));*/
            return services;
        }
    }
}
