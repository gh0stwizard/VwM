using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace VwM.Models.API.DataTables
{
    [DataContract, JsonObject]
    public class Order
    {
        [DataMember(Name = "column", IsRequired = true), JsonProperty(PropertyName = "column")]
        public int Column { get; set; }

        [DataMember(Name = "dir", IsRequired = true), JsonProperty(PropertyName = "dir")]
        public string Direction { get; set; }
    }
}
