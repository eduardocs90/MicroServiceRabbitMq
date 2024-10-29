using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
namespace NotificationService.NotificationService
{
    public class NotificationService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public NotificationService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declaração de fila para notificações
            _channel.QueueDeclare(queue: "notification_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var notificationEvent = JsonConvert.DeserializeObject<NotificationEvent>(message);

                // Simula o envio de notificação ao usuário
                Console.WriteLine($"Notification for account {notificationEvent.AccountNumber}: {notificationEvent.Message}");
            };

            _channel.BasicConsume(queue: "notification_queue", autoAck: true, consumer: consumer);
        }
    }

    public class NotificationEvent
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}