using Microsoft.Extensions.Configuration;

namespace stock_dotnet.utils.env
{
    public class Env : IEnv
    {
        public int Port { get; set; }
        public string RabbitURL { get; set; }
        public string RabbitUser { get; set; }
        public string RabbitPasword { get; set; }
        public string SecurityServerURL { get; set; }

    }

    public interface IEnv
    {
        int Port { get; set; }
        string RabbitURL { get; set; }
        string RabbitUser { get; set; }
        string RabbitPasword { get; set; }
        string SecurityServerURL { get; set; }


    }
    // public class Env
    // {
    //     private IConfiguration _configuration { get; }

    //     public Env(IConfiguration configuration)
    //     {
    //         _configuration = configuration;
    //     }

    //     public string Get(string key)
    //     {
    //         var cnf =  _configuration.GetSection("configuration:" + key).Value;
    //         return cnf;
    //     }


    // }
}