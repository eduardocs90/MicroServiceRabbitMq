using Shared;
using WebApplicationNotifyer.Consumer;
using WebApplicationNotifyer.Interface;
using WebApplicationNotifyer.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Carregar configurações de RabbitMQ
builder.Configuration.GetSection("RabbitMq");

// Configurar o serviço de RabbitMQ
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHostedService<RabbitConsumer>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
