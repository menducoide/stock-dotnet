
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
using stock_dotnet.stock.vo;
using System;

namespace stock_dotnet.utils.rabbitmq
{
    public class ConsumeReserveArticletService : BackgroundService
    {
        private readonly ILoggerManager _logger;
        private IConnection _connection;
        private IModel _channel;

        private readonly EmiterRabbit _emiterRabbit;
        private readonly StockService _stockService;

        private IEnv _env;
        public string queueName { get; set; }

        public RabbitConfiguration configuration { get; set; }

        public ConsumeReserveArticletService(ILoggerManager logger, IEnv env, StockService stockService, EmiterRabbit emiterRabbit)
        {
            configuration = new RabbitConfiguration(
                                     exchange: Const.EXCHANGE_ARTICLE_RESERVED,
                                     queue: "",
                                     exchangeType: ExchangeType.Fanout
                                 );
            _logger = logger;
            _stockService = stockService;
            _env = env;
            _emiterRabbit = emiterRabbit;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = _env.RabbitURL };

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
            ResponseRabbitGeneric response = JsonConvert.DeserializeObject<ResponseRabbitGeneric>(message);
            VoReservation voReservation = new VoReservation
            {
                Quantity = response.message.quantity,
                ArticleId = response.message.articleId
            };
            try
            {
                string reservationId = _stockService.Reserve(voReservation);

                var result = new
                {
                    type = Const.EXCHANGE_STOCK_RESERVED,
                    message = new
                    {
                        reservationId = reservationId
                    }
                };
                _emiterRabbit.Emit(new RabbitConfiguration
                {
                    Exchange = Const.EXCHANGE_STOCK_RESERVED,
                    ExchangeType = ExchangeType.Fanout,
                    Queue = ""
                }, JsonConvert.SerializeObject(result));

            }
            catch (Exception e)
            {
                var result = new
                {
                    type = Const.EXCHANGE_STOCK_RESERVED,
                    message = new
                    {
                        error = e.Message
                    }
                };
                _emiterRabbit.Emit(new RabbitConfiguration
                {
                    Exchange = Const.EXCHANGE_STOCK_RESERVED,
                    ExchangeType = ExchangeType.Fanout,
                    Queue = ""
                }, JsonConvert.SerializeObject(result));
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