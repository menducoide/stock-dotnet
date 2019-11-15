using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
namespace stock_dotnet.stock
{
    public class StockDetail
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReservationId { get; set; }
        public DateTime? ReserveDate { get; set; }
     
        public bool? Confirmed { get; set; }
    }


}