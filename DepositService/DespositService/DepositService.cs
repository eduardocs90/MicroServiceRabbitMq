using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

public class DepositService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public DepositService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declaração de fila para eventos de depósitos
        _channel.QueueDeclare(queue: "deposit_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void DepositMoney(decimal amount, string accountNumber)
    {
        var depositEvent = new
        {
            Amount = amount,
            AccountNumber = accountNumber,
            Timestamp = DateTime.Now
        };

        var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(depositEvent));

        // Publica o evento para a fila de processamento
        _channel.BasicPublish(exchange: "", routingKey: "deposit_queue", basicProperties: null, body: messageBody);

        Console.WriteLine($"Deposit event published: Account {accountNumber}, Amount {amount}");
    }
}
