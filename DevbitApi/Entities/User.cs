using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevbitApi.Entities
{
    public class User
    {
        public User(string json)
        {
            JObject jObject = JObject.Parse(json);
            JToken jUser = jObject[""];
            id = (string)jUser["id"];
            UserName = (string)jUser["UserName"];
            UserPassword = (string)jUser["UserPassword"];
            Email = (string)jUser["Email"];
        }

        public string id { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Email { get; set; }
    }
}
