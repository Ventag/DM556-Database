using System;
using System.Runtime.Serialization;
using MongoDB.Bson;

namespace Database.Model
{
    [DataContract]
    public class RatingInfo
    {
        [DataMember(Name = "_id")]
        public ObjectId UserId { get; set; }

        [DataMember(Name = "drinkid")]
        public string DrinkId { get; set; }

        [DataMember(Name = "rating")]
        public int Rating { get; set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }

        [DataMember(Name = "helpfull")]
        public int Helpfull { get; set; }

        [DataMember(Name = "unhelpfull")]
        public int Unhelpfull { get; set; }

    }
}
