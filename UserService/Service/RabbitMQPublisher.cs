using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

namespace UserService.Service
{

    public class RabbitMQPublisher : IMessagePublisher
    {
        // Armazena o host do RabbitMQ
        private readonly string _host;

        // Nome da fila onde as mensagens serão publicadas
        private readonly string _queueName;

        // Construtor que recebe a configuração da aplicação para inicializar as variáveis
        public RabbitMQPublisher(IConfiguration configuration)
        {
            // Obtém o host do RabbitMQ a partir do arquivo de configuração (appsettings.json)
            _host = configuration["RabbitMQ:Host"];

            // Obtém o nome da fila a partir do arquivo de configuração
            _queueName = configuration["RabbitMQ:QueueName"];
        }

        // Método que publica uma mensagem de usuário criado no RabbitMQ
        public void PublishUserCreated(UserNotification notification)
        {
            // Cria a fábrica de conexão para se conectar ao RabbitMQ usando o host configurado
            var factory = new ConnectionFactory() { HostName = _host };

            // Cria a conexão com o RabbitMQ
            using var connection = factory.CreateConnection();

            // Cria um canal de comunicação no RabbitMQ
            using var channel = connection.CreateModel();

            // Declara a fila onde a mensagem será publicada
            channel.QueueDeclare(queue: _queueName,   // Nome da fila
                                 durable: false,     // A fila não é persistente
                                 exclusive: false,   // A fila não é exclusiva
                                 autoDelete: false,  // A fila não será deletada automaticamente
                                 arguments: null     // Sem argumentos adicionais
            );

            // Serializa o objeto de notificação (UserNotification) em uma string JSON
            var message = JsonSerializer.Serialize(notification);

            // Converte a mensagem JSON para um array de bytes (necessário para enviar pelo RabbitMQ)
            var body = Encoding.UTF8.GetBytes(message);

            // Publica a mensagem no RabbitMQ, sem especificar uma exchange e usando a fila como rota
            channel.BasicPublish(exchange: "",        // Não usa exchange
                                 routingKey: _queueName,  // Roteia diretamente para a fila
                                 basicProperties: null,   // Sem propriedades adicionais
                                 body: body               // Conteúdo da mensagem
            );
        }
    }

}
