using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Text;
using WebApplicationNotifyer.Interface;

namespace WebApplicationNotifyer.Consumer
{
    public class RabbitConsumer : BackgroundService
    {
        // Configuração do RabbitMQ que contém informações como host e fila
        private readonly RabbitMqConfiguration _config;

        // Conexão com o RabbitMQ
        private readonly IConnection _connection;

        // Canal do RabbitMQ para realizar operações como envio e recebimento de mensagens
        private readonly IModel _channel;

        // Serviço para resolver dependências, como o serviço de notificação
        private readonly IServiceProvider _serviceProvider;

        // Construtor que inicializa as variáveis e cria a conexão com o RabbitMQ
        public RabbitConsumer(IOptions<RabbitMqConfiguration> options, IServiceProvider serviceProvider)
        {
            // Atribui as configurações do RabbitMQ passadas por injeção de dependência
            _config = options.Value;

            // Serviço para criar escopos de serviços (resolver dependências)
            _serviceProvider = serviceProvider;

            // Cria a fábrica de conexão com base nas configurações do RabbitMQ
            var factory = new ConnectionFactory
            {
                HostName = _config.Host // Define o host do RabbitMQ
            };

            // Cria uma conexão com o RabbitMQ
            _connection = factory.CreateConnection();

            // Cria um canal de comunicação (usado para enviar/receber mensagens)
            _channel = _connection.CreateModel();

            // Declara a fila onde as mensagens serão consumidas
            _channel.QueueDeclare(
                queue: _config.Queue,  // Nome da fila
                durable: false,        // A fila não é persistente
                exclusive: false,      // A fila não é exclusiva (pode ser acessada por várias conexões)
                autoDelete: false,     // A fila não será deletada automaticamente
                arguments: null        // Não há argumentos adicionais
            );
        }

        // Método chamado quando o serviço é iniciado; será executado em background
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Cria um consumidor que ficará escutando a fila
            var consumer = new EventingBasicConsumer(_channel);

            // Evento disparado quando uma nova mensagem for recebida
            consumer.Received += (sender, eventArgs) =>
            {
                // Converte o corpo da mensagem (byte array) para uma string UTF-8
                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);

                // Desserializa a string JSON para o objeto UserNotification
                var message = JsonConvert.DeserializeObject<UserNotification>(contentString);

                // Chama o método que irá notificar o usuário (enviar o e-mail)
                NotifyUser(message);

                // Confirma que a mensagem foi processada com sucesso (acknowledgment)
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            // Inicia o consumo da fila, definindo que o `autoAck` está desativado (o consumidor deve confirmar manualmente)
            _channel.BasicConsume(_config.Queue, false, consumer);

            // Retorna uma tarefa concluída, indicando que o método de background foi iniciado
            return Task.CompletedTask;
        }

        // Método responsável por notificar o usuário via e-mail
        public void NotifyUser(UserNotification message)
        {
            // Cria um escopo de serviço para resolver as dependências
            using (var scope = _serviceProvider.CreateScope())
            {
                // Obtém a instância do serviço de notificação (pode ser um serviço de e-mail)
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // Chama o serviço para enviar o e-mail de notificação
                notificationService.NotifyerUser(
                    message.Email, 
                    "Cadastro Realizado",// E-mail do destinatário
                  $"Olá Seja bem vindo {message.Nome},seu saldo é de: {message.Saldo}, atualizado em: {message.DataCriacao}"
                );
            }
        }
    }

}
