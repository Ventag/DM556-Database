using System.Runtime.Serialization;

namespace Database.Model
{
    public class UserInfo
    {
        [DataMember(Name = "_id")]
        public string Id { get; set; }
    }
}
