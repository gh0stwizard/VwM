using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace VwM.Models.API.DataTables
{
    /// <summary>
    /// Reference: https://datatables.net/manual/server-side
    /// </summary>

    [DataContract, JsonObject]
    public class Request
    {
        [DataMember(Name = "draw"), JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }

        [DataMember(Name = "start"), JsonProperty(PropertyName = "start")]
        public int Start { get; set; }

        [DataMember(Name = "length"), JsonProperty(PropertyName = "length")]
        public int Length { get; set; }

        [DataMember(Name = "search"), JsonProperty(PropertyName = "search")]
        public Search Search { get; set; }

        [DataMember(Name = "order"), JsonProperty(PropertyName = "order")]
        public List<Order> Order { get; set; }

        [DataMember(Name = "columns"), JsonProperty(PropertyName = "columns")]
        public List<Column> Columns { get; set; }
    }
}
