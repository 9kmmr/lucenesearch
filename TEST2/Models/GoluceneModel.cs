using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net;
using System.IO;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.QueryParsers;
using Lucene.Net.Analysis;
using Lucene.Net.Search.Highlight;




namespace TEST2.Models
{
    public static class GoluceneModel
    {
        public static  int hitsPerPage = 10;
        public static string _luceneDir =
            Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index");
                    private static FSDirectory _directoryTemp;
                    private static FSDirectory _directory
                    {
                        get
                        {
                            if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                            if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                            var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                            if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                            return _directoryTemp;
                        }
                    }

        // search methods
        //public static List<SiteObject> GetAllIndexRecords()
        //{
        //    // validate search index
        //    if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<SiteObject>();

        //    // set up lucene searcher
        //    var searcher = new IndexSearcher(_directory, false);
        //    var reader = IndexReader.Open(_directory, false);
        //    var docs = new List<Document>();
        //    var term = reader.TermDocs();
        //    // v 2.9.4: use 'hit.Doc()
        //    // v 3.0.3: use 'hit.Doc'
        //    while (term.Next()) docs.Add(searcher.Doc(term.Doc));
        //    reader.Dispose();
        //    searcher.Dispose();
        //    return _mapLuceneToDataList(docs);
        //}
        // search with pagination
        public static ResultObject Search(string input,int pad)
        {
            if (string.IsNullOrEmpty(input)) return new ResultObject();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input,pad);
        }
        public static ResultObject SearchDefault(string input,int pad, string fieldName = "")
        {
            return string.IsNullOrEmpty(input) ? new ResultObject() : _search(input,pad, fieldName);
        }
        // main search method
        private static ResultObject _search(string searchQuery,int pad, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new ResultObject();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 10000;
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);

                int start = (pad - 1) * hitsPerPage;
                int end = start + hitsPerPage;

                
                var parser = new MultiFieldQueryParser
                        (Version.LUCENE_30, new[] { "Contents", "Title" }, analyzer);
                var query = parseQuery(searchQuery, parser);
                // return all scoredoc for all searched document
                searcher.SetDefaultFieldSortScoring(true, true);
                var sort = new Sort(new SortField("Contents", SortField.SCORE));
                var hits = searcher.Search(query, null, hits_limit, sort).ScoreDocs;
                   
                ResultObject results = new ResultObject();
                List<CustomresultObject> listsite = new List<CustomresultObject>();
                for (int i = start; i < Math.Min(end, hits.Length); ++i)
                    {
                        listsite.Add(_mapLuceneToDataList(hits[i], searcher,query, analyzer));                        
                    }
                results.sites = listsite;
                results.NumberResult = hits.Length;
                results.Resultperpage = hitsPerPage;
                analyzer.Close();
                searcher.Dispose();
                // WRITE LOG
                string datenow = DateTime.Now.ToString("h:mm:ss tt");
                string path = @"D:\MyTest.txt";
                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    string createText = "Time: "+datenow +"|query: "+ searchQuery+"|Resultnum: "+ hits.Length + Environment.NewLine;
                    File.WriteAllText(path, createText);
                }

                // This text is always added, making the file longer over time
                // if it is not deleted.
                string appendText =  "Time: " + datenow + "|query: " + searchQuery + "|Resultnum: " + hits.Length + Environment.NewLine;
                File.AppendAllText(path, appendText);


                return results;
               
            }
        }
        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }
        // map Lucene search index to data
        //private static List<SiteObject> _mapLuceneToDataList(IEnumerable<Document> hits)
        //{
        //    return hits.Select(_mapLuceneDocumentToData).ToList();
        //}
        private static CustomresultObject _mapLuceneToDataList(ScoreDoc hit, IndexSearcher searcher,Query query, StandardAnalyzer analys)
        {
            // v 2.9.4: use 'hit.doc'
            // v 3.0.3: use 'hit.Doc'
           
            //return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc), query,analys,searcher,hit)).ToList();
            return _mapLuceneDocumentToData(searcher.Doc(hit.Doc), query, analys, searcher, hit);
        }
        private static CustomresultObject _mapLuceneDocumentToData(Document doc,Query query, StandardAnalyzer analys, IndexSearcher searcher , ScoreDoc hit)
        {

            string text = doc.Get("Contents");
            string[] highlighted = TextHighlighter( query, text,  analys,  searcher,hit);
            string contentresult = string.Join("\r\n", highlighted);
            
            return new CustomresultObject
            {
                url = doc.Get("Url"),
                contents = contentresult,
                title = doc.Get("Title")                           
            };
        }
        /** GET HIGHLIGHTER FRAGMENT*/
        
        public static string[] TextHighlighter(Query query, string text, StandardAnalyzer analys, IndexSearcher searcher,ScoreDoc doc)
        {
            QueryScorer scorer = new QueryScorer(query);
            SimpleHTMLFormatter formater = new SimpleHTMLFormatter("<b>", "</b>");
            Highlighter highlighter = new Highlighter(formater, scorer);
            TokenStream  tokenStream = TokenSources.GetAnyTokenStream(searcher.IndexReader, doc.Doc, "Contents", analys);
            string[] frags = highlighter.GetBestFragments(tokenStream,text,3);
            return frags;

        }
        
        private static void _addToLuceneIndex(SiteObject SiteObject, IndexWriter writer)
        {
            // add new index entry
            var doc = new Document();
                    
            // add lucene fields mapped to db fields             

            string url = SiteObject.url;
            string contents = SiteObject.content;
            int id = SiteObject.id;            
            string title = SiteObject.title;
            doc.Add(new Field("Id", id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Url", url, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Contents", contents, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Title", title, Field.Store.YES, Field.Index.ANALYZED));
           
            // add entry to index
            writer.AddDocument(doc);
        }
        // add a single site object
        public static void AddUpdateLuceneIndex(SiteObject SiteObject)
        {
            AddUpdateLuceneIndex(new List<SiteObject> { SiteObject });
        }
        // add list site object
        public static void AddUpdateLuceneIndex(List<SiteObject> SiteList)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // add data to lucene search index (replaces older entries if any)
                foreach (var SiteObject in SiteList) _addToLuceneIndex(SiteObject, writer);

                // close handles
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }
       
        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }

    }
}