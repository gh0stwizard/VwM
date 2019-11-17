using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace VwM.Models.API.DataTables
{
    [DataContract, JsonObject]
    public class Column
    {
        [DataMember(Name = "data", IsRequired = true), JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        [DataMember(Name = "name", IsRequired = true), JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember(Name = "searchable", IsRequired = true), JsonProperty(PropertyName = "searchable")]
        public bool Searchable { get; set; }

        [DataMember(Name = "orderable", IsRequired = true), JsonProperty(PropertyName = "orderable")]
        public bool Orderable { get; set; }

        [DataMember(Name = "search"), JsonProperty(PropertyName = "search")]
        public Search Search { get; set; }
    }
}
