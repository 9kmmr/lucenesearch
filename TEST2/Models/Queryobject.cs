using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEST2.Models
{
    public class Queryobject
    {
        
        public List<string> authors { get; set; }
        
        public string description { get; set; }
      
        public int id { get; set; }
       
        public string modified { get; set; }
        
        public string query { get; set; }
    }
}