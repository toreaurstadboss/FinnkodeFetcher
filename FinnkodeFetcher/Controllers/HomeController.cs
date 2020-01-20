using FinnkodeFetcher.Models;
using RestSharp;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using FinnkodeFetcher.Common;

namespace FinnkodeFetcher.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var searchResultsModel = new SearchResultsDataContract();

            return View(searchResultsModel);
        }

        [HttpPost]
        public PartialViewResult SearchIcd10(string code)
        {
            var client = new RestClient("https://finnkode.ehelse.no/FinnKodeWS/SearchService.svc/CommitSearch?_dc=1579468909297");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"searchExpression\":\"" + code + "\",\"sources\":[{\"SearchSourceId\":\"ICD10SysDel\",\"Selected\":true}],\"page\":1,\"start\":0,\"limit\":25}",  ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            var result = new SearchResultsDataContract();
            
            var xmlDoc = XDocument.Parse(response.Content);
            //var jsonDoc = JsonConvert.SerializeObject(xmlDoc);

            if (xmlDoc.Root != null)
            {
                var records = xmlDoc.Root.Elements("record");
                foreach (var record in records)
                {
                    var item = new SearchResultDataContract
                    {
                        Code = record.Element("code")?.Value,
                        Text = record.Element("title")?.Value
                    };
                    if (string.IsNullOrEmpty(item.Code) || string.IsNullOrEmpty(item.Text) || item.Code.Contains("(")
                        || !item.Code.StartsWith(code, StringComparison.InvariantCultureIgnoreCase) || !item.Code.Contains("."))
                    {
                        continue;
                    }
                    //replace the "." sign
                    item.Code = item.Code.Replace(".", "");
                    result.SearchResults.Add(item);
                }
                result.Code = code;
            }

            if (result.SearchResults.Any())
            {
                result.SearchResults = result.SearchResults.OrderBy(item => item.Code, new AlphanumericComparator<string>()).ToList();

            }
            return PartialView("_SearchResults", result);
        }

    }
}
