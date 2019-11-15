using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace stock_dotnet.stock.vo
{
    public class VoStock
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
        public DateTime? LastModification { get; set; }

        [BsonRepresentation(BsonType.ObjectId)] 
        public string ArticleId { get; set; }
    }
}
