using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using TEST2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TEST2.Controllers
{
    public class SearchController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(string query,int pad)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // perform Lucene search
            ResultObject _searchResults;            
            _searchResults = GoluceneModel.Search(query, pad);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            _searchResults.Time = elapsedMs;
            string json = JsonConvert.SerializeObject(_searchResults);
            return new HttpResponseMessage()
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
           
        }
        [System.Web.Http.HttpGet]
        public void createindex(string create)
        {
            if (create.Equals("create")){
                CollectionObject CollectionList = new ReadJsonModel().Readallfileseach();
                
                foreach (var collection in CollectionList.documents)
                    {
                        GoluceneModel.AddUpdateLuceneIndex(collection);
                    }
                
            }
        }
        // POST api/values
        [System.Web.Http.HttpGet]
        public void del(string delete)
        {
            if (delete.Equals("deleteindex"))
            {
                GoluceneModel.ClearLuceneIndex();
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        public void IndexSearch
      (string searchTerm, string searchField,int pad, bool? searchDefault, int? limit)
        {
            // create default Lucene search index directory
            if (!Directory.Exists(GoluceneModel._luceneDir)) Directory.CreateDirectory(GoluceneModel._luceneDir);

            

            // setup and return view model

            // limit display number of database records

            
        }

        
    }
}
