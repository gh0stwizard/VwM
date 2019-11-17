using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace VwM.Models.API.DataTables
{
    [DataContract, JsonObject]
    public class Response
    {
        [DataMember(Name = "draw"), JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }

        [DataMember(Name = "recordsTotal"), JsonProperty(PropertyName = "recordsTotal")]
        public int Total { get; set; }

        [DataMember(Name = "recordsFiltered"), JsonProperty(PropertyName = "recordsFiltered")]
        public int Filtered { get; set; }

        [DataMember(Name = "data"), JsonProperty(PropertyName = "data")]
        public object Data { get; set; }

        [DataMember(Name = "error"), JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}
