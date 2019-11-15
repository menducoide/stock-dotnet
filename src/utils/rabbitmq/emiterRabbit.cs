
using System.Text;
using RabbitMQ.Client;

namespace stock_dotnet.utils.rabbitmq
{
    public class EmiterRabbit
    {
        public void Emit(RabbitConfiguration configuration, string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: configuration.Exchange, type: configuration.ExchangeType);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: configuration.Exchange,
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);
            }
        }

    }
}