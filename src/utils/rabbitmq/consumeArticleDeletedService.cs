
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using stock_dotnet.utils.env;
using Microsoft.Extensions.Caching.Memory;
using stock_dotnet.utils.cache;
using stock_dotnet.stock;
using System;

namespace stock_dotnet.utils.rabbitmq
{
    public class ConsumeArticleDeletedService : BackgroundService
    {
        private readonly ILoggerManager _logger;
        private IConnection _connection;
        private IModel _channel;

        private readonly StockService _stockService;
        public string queueName { get; set; }

        public RabbitConfiguration configuration { get; set; }

        public ConsumeArticleDeletedService(ILoggerManager logger, StockService stockService)
        {
            configuration = new RabbitConfiguration(
                                     exchange: Const.EXCHANGE_ARTICLE_DELETED,
                                     queue: "",
                                     exchangeType: ExchangeType.Fanout
                                 );
            _logger = logger;
            _stockService = stockService;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            // create connection  
            _connection = factory.CreateConnection();
            // create channel  
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: configuration.Exchange, type: configuration.ExchangeType);

            queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName,
                              exchange: configuration.Exchange,
                              routingKey: "");
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body);

                // handle the received message  
                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(queueName, false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string message)
        {
            try
            {
                ResponseRabbitGeneric response = JsonConvert.DeserializeObject<ResponseRabbitGeneric>(message);
                 string articleId = response.message.article_id;
                _stockService.Delete(articleId);
                // we just print this message   
                _logger.LogInfo($"Article: {articleId} was deleted");

            }
            catch (Exception e)
            {
                    _logger.LogError($"Article from message: {message} can't deleted /b {e.Message}");
            }
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}