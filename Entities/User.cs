using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APiTest.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        
        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}
