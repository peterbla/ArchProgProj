using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class UserCredentials
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class UserWithToken
    {
        [JsonPropertyName("user")]
        public NewUser User { get; set; }
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
    public class NewUser
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int? HeightCm { get; set; }
        public float? WeightKg { get; set; }
    }
}
