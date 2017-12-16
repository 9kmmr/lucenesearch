using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEST2.Models
{
    public class RelevancesObject
    {
        
        public List<string> authors { get; set; }
        
        public int docid { get; set; }
        
        public int id { get; set; }
        
        public string modified { get; set; }
        
        public int queryid { get; set; }
       
        public Boolean yes { get; set; }

    }
}