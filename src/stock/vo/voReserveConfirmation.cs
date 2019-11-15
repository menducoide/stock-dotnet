using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace stock_dotnet.stock.vo
{
    public class VoReserveConfirmation
    {
        [BsonRepresentation(BsonType.ObjectId)] 
        public string ReservationId { get; set; }
    }
}
