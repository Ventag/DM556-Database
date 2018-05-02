using System;
using System.Runtime.Serialization;

namespace Database.Model
{
    [DataContract]
    public class DrinkCombinations
    {
        [DataMember]
        public Guid Id { get; set; }


        public DrinkCombinations()
        {
        }
    }
}
