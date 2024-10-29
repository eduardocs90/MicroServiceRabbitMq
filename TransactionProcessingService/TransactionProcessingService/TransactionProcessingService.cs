using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class TransactionProcessingService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public TransactionProcessingService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declaração de fila de eventos de depósitos
        _channel.QueueDeclare(queue: "deposit_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void StartProcessing()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var depositEvent = JsonConvert.DeserializeObject<DepositEvent>(message);

            Console.WriteLine($"Processing deposit for account {depositEvent.AccountNumber}: {depositEvent.Amount}");

            // Aqui você faria a lógica de atualização do saldo da conta, etc.

            // Após o processamento, envia um evento de notificação para RabbitMQ
            SendNotification(depositEvent);
        };

        _channel.BasicConsume(queue: "deposit_queue", autoAck: true, consumer: consumer);
    }

    private void SendNotification(DepositEvent depositEvent)
    {
        var notificationEvent = new
        {
            AccountNumber = depositEvent.AccountNumber,
            Amount = depositEvent.Amount,
            Timestamp = DateTime.Now,
            Message = $"Deposit of {depositEvent.Amount} confirmed."
        };

        var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(notificationEvent));

        // Publica o evento de notificação
        _channel.BasicPublish(exchange: "", routingKey: "notification_queue", basicProperties: null, body: messageBody);

        Console.WriteLine($"Notification sent for account {depositEvent.AccountNumber}");
    }
}

public class DepositEvent
{
    public decimal Amount { get; set; }
    public string AccountNumber { get; set; }
    public DateTime Timestamp { get; set; }
}
