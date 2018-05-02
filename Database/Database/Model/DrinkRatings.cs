using System;
using System.Runtime.Serialization;

namespace Database.Model
{
    [DataContract]
    public class DrinkRatings
    {
        [DataMember(Name = "userId")]
        public string UserId { get; set; }

        [DataMember(Name = "drinkId")]
        public Guid DrinkId { get; set; }

        [DataMember(Name = "rating")]
        public int Rating { get; set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }

        [DataMember(Name = "helpfull")]
        public int Helpfull { get; set; }

        [DataMember(Name = "unhelpfull")]
        public int Unhelpfull { get; set; }

        public DrinkRatings()
        {
        }
    }
}
