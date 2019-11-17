using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace VwM.Models.API.DataTables
{
    [DataContract, JsonObject]
    public class Search
    {
        [DataMember(Name = "value", IsRequired = true), JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [DataMember(Name = "regex", IsRequired = true), JsonProperty(PropertyName = "regex")]
        public bool Regex { get; set; }

        public override string ToString() => Value;
    }
}
