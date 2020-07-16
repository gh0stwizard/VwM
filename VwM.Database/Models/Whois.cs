using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VwM.Database.Models
{
    public class Whois
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("Created")]
        public DateTime Created { get; set; }


        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("Updated")]
        public DateTime Updated { get; set; }


        [BsonElement("Hostname")]
        public string Hostname { get; set; }


        [BsonElement("Result")]
        public string Result { get; set; }
    }
}
