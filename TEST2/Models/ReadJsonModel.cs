using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace TEST2.Models
{
    public class ReadJsonModel
    {
        // get each file json in directory 
        public CollectionObject Readallfileseach()
        {           
            string allcontent = File.ReadAllText("C:\\Users\\admin\\Desktop\\working_file\\it4853dataset\\all.json");            
            CollectionObject collectionObject = parseJSONFileeach(allcontent);           
            return collectionObject;
        }
       
        // convert each json file to collectionobject
        public CollectionObject parseJSONFileeach(string json)
        {
            return JsonConvert.DeserializeObject<CollectionObject>(json);
            
        }
    }
}