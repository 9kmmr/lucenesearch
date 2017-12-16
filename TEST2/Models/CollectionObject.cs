using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEST2.Models
{
    public class CollectionObject
    {
        [JsonProperty("combined")]
        public string combined { get; set; }
        [JsonProperty("documents")]
        public List<SiteObject> documents { get; set; }
        [JsonProperty("queries")]
        public List<Queryobject> queries { get; set; }
        [JsonProperty("relevances")]
        public List<RelevancesObject> relevances { get; set; }
    }
}