using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserType
    {
        Admin = 1,
        User = 2
    }
}
