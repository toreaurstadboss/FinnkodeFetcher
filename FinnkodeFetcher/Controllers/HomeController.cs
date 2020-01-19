using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using FinnkodeFetcher.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Linq;

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

            var xmlDoc = XDocument.Parse(response.Content);
            //var jsonDoc = JsonConvert.SerializeObject(xmlDoc);

            var records = xmlDoc.Root.Elements("record");
            var result = new SearchResultsDataContract();
            foreach (var record in records)
            {
                var item = new SearchResultDataContract
                {
                    Code = record.Element("code").Value,
                    Text = record.Element("title").Value
                };
                result.SearchResults.Add(item);
            }

            result.Code = code;
            return PartialView("_SearchResults", result);
        }
    }
}
