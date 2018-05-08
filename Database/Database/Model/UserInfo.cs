using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Database.Model
{
    public class UserInfo
    {
        [DataMember(Name = "_id")]
        public string Id { get; set; }

        public UserInfo()
        {
            
        }
    }
}
