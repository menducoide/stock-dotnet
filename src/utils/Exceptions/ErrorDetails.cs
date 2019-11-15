
using Newtonsoft.Json;

namespace stock_dotnet.utils.Exceptions
{
 public class ErrorDetails
    {
        
        public int StatusCode { get; set; }
        public object Message { get; set; }
 
 
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}