using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEST2.Models;
using System.IO;
namespace TEST2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Bello My Friend";
            // create default Lucene search index directory
            if (!Directory.Exists(GoluceneModel._luceneDir)) Directory.CreateDirectory(GoluceneModel._luceneDir);

            return View();
        }
    }
}
