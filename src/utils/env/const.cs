
namespace stock_dotnet.utils.env
{
    public static class Const
    {
        public static int Reservation_Expiration_Time = 1;
        public static int CACHE_DEFAULT_EXPIRATION_MINUTES = 10;
        public static string EXCHANGE_AUTH = "auth";
        public static string EXCHANGE_ARTICLE_DELETED = "article-deleted";
        public static string EXCHANGE_ARTICLE_RESERVED = "article-reserved";
        public static string EXCHANGE_STOCK_RESERVED = "stock-reserved";
        
        public static string NotFoundException_Message = "The stock for the article: {0} is not found";

        public static string BusinessException_Stock_Not_Available = "The quantity to reserve isn't avaliable";
        public static string BusinessException_Reservation_Not_Available = "The reserve isn't avaliable";
        public static string BusinessException_CANT_CREATE = "The stock for the article: {0} is already existent";



    }
}