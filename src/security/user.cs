namespace stock_dotnet.security
{
    public class User
    {
        public string name { get; set; }
        public string[] permissions { get; set; }
        public string login { get; set; }
    }
}