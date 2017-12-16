using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEST2.Models
{
    public class ResultObject
    {
        public int NumberResult { get; set; }
        public int Resultperpage { get; set; }
        public List<CustomresultObject> sites { get; set; }
        public long Time { get; set; }
    }
}