using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.NotificationService;
using Shared;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Instanciando RabbitMqConfiguration manualmente
        var rabbitMqConfig = new RabbitMqConfiguration();
        Configuration.GetSection("RabbitMQ").Bind(rabbitMqConfig);
        services.AddSingleton(rabbitMqConfig); // Registrando como singleton

        // Registre outros serviços que você deseja usar
        services.AddHostedService<RabbitConsumer>(); // Se você tiver um consumidor
    }
}
