using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using stock_dotnet.utils.env;
using stock_dotnet.utils.rabbitmq;

namespace stock_dotnet
{
    public class Program
    {
        public static void Main(string[] args)
        {
        //    RunRabbit();
            //  CreateWebHostBuilder(args).Build().Run();
            var host = new WebHostBuilder()
      .UseKestrel()
      .UseContentRoot(Directory.GetCurrentDirectory())
      .UseIISIntegration()
      .UseStartup<Startup>()
      .Build();

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static void RunRabbit()
        {
            new Thread(() =>
                        {
                            var config = new RabbitConfiguration(
                            exchange: Const.EXCHANGE_AUTH,
                            queue: "",
                            exchangeType: ExchangeType.Fanout
                        );
                            var factory = new ConnectionFactory() { HostName = "localhost" };
                            using (var connection = factory.CreateConnection())
                            using (var channel = connection.CreateModel())
                            {
                                channel.ExchangeDeclare(exchange: config.Exchange, type: config.ExchangeType);

                                var queueName = channel.QueueDeclare().QueueName;
                                channel.QueueBind(queue: queueName,
                                      exchange: config.Exchange,
                                      routingKey: "");


     Console.WriteLine(" [x] {0}", "Waiting for messages");
                                var consumer = new EventingBasicConsumer(channel);
                                consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] {0}", message);
                    };
                                channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                            }
                        }).Start();
        }
    }
}
