using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEST2.Models
{
    public class SiteObject
    {
        
        public List<string> authors { get; set; }
      
        public string content { get; set; }
      
        public int id { get; set; }
       
        public string modified { get; set; }
       
        public string title { get; set; }
        
        public string url { get; set; }
    }
}