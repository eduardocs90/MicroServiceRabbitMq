using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json; // Certifique-se de que este namespace está presente
using Shared;
using Microsoft.Extensions.Configuration; // Para a classe UserNotification

namespace NotificationService.NotificationService
{
    public class RabbitMQConsumer
    {
        private readonly string _host;
        private readonly string _queueName;

        public RabbitMQConsumer(IConfiguration configuration)
        {
            _host = configuration["RabbitMQ:Host"];
            _queueName = configuration["RabbitMQ:QueueName"];
        }

        public void Consume()
        {
            var factory = new ConnectionFactory() { HostName = _host };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var notification = JsonSerializer.Deserialize<UserNotification>(message); // Certifique-se de que a classe está acessível
                SendEmail(notification);
            };

            channel.BasicConsume(queue: _queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Listening for messages. Press [enter] to exit.");
            Console.ReadLine();
        }

        private void SendEmail(UserNotification notification)
        {
            // Lógica para enviar o e-mail
            string toEmail = notification.Email;
            string subject = "Cadastro com sucesso!";
            string body = notification.Message;

            // Aqui você pode usar uma biblioteca de envio de e-mail como SmtpClient ou uma API de e-mail
            Console.WriteLine($"Enviando e-mail para {toEmail} com assunto: {subject} e corpo: {body}");
        }
    }
}
