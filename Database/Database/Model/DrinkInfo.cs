using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;
        
namespace Database.Model
{
    [DataContract]
    public class DrinkInfo
    {
        [DataMember(Name = "_id")]
        public string Id { get; set; }

        [DataMember(Name = "userId")]
        public string UserId { get; set; }

        [DataMember(Name = "gin")]
        public string Gin { get; set; }

        [DataMember(Name = "tonic")]
        public string Tonic { get; set; }

        [DataMember(Name = "garnish")]
        public string Garnish { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
