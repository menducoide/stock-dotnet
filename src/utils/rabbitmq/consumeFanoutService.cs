
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using stock_dotnet.utils.env;
using Microsoft.Extensions.Caching.Memory;
using stock_dotnet.utils.cache;

namespace stock_dotnet.utils.rabbitmq
{
    public class ConsumeFanoutService : BackgroundService
    {
        private readonly ILoggerManager _logger;
        private IConnection _connection;
        private IModel _channel;
        private ICacheHandler _cache;
        public string queueName { get; set; }

        public RabbitConfiguration configuration { get; set; }

        public ConsumeFanoutService(ILoggerManager logger,ICacheHandler cache)
        {
            configuration = new RabbitConfiguration(
                                     exchange: Const.EXCHANGE_AUTH,
                                     queue: "",
                                     exchangeType: ExchangeType.Fanout
                                 );
            _logger = logger;
            _cache = cache;
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
            ResponseFanout response = JsonConvert.DeserializeObject<ResponseFanout>(message);
            _cache.Invalidate(response.message.Substring(7));
            // we just print this message   
            _logger.LogInfo($"consumer received {message}");
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