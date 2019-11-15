
namespace stock_dotnet.utils.rabbitmq{
    public class RabbitConfiguration{
        public string Exchange { get; set; }
        public string Queue { get; set; }
        public string ExchangeType { get; set; }

        public RabbitConfiguration(string exchange, string queue, string exchangeType)
        {
            Exchange = exchange;
            Queue = queue;
            ExchangeType = exchangeType;
        }

        public RabbitConfiguration(){}
    }
}