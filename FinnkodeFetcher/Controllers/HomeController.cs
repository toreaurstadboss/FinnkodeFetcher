using FinnkodeFetcher.Models;
using RestSharp;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using FinnkodeFetcher.Common;
using FinnkodeFetcher.DbContext;
using FinnkodeFetcher.DbContext.Models;

namespace FinnkodeFetcher.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "ICD-10 Søk";
            var searchResultsModel = new SearchResultsDataContract();
            return View(searchResultsModel);
        }

        public ActionResult Harvest()
        {
            ViewBag.Title = "ICD-10 Høsting";
            var harvestResultsModel = new HarvestResultsDataContract();
            return View(harvestResultsModel);
        }

        public string HarvestIcd10(string prefix)
        {
            string reply = HarvestLogic(prefix);
            return reply;
        }

        private static string HarvestLogic(string prefix)
        {
            string reply;
            if (string.IsNullOrEmpty(prefix) || prefix.Length < 2)
            {
                reply = "No icd10 codes found. Wrong request. Icd10 code prefix must be at least string length 2";
                return reply;
            }

            var result = GetIcd10Codes(prefix);
            if (result == null || result.SearchResults == null || !result.SearchResults.Any())
            {
                reply = "No icd10 codes found with prefix: " + prefix;
                return reply;
            }

            using (var dbContext = new FinnkodeFetcherDbContext())
            {
                foreach (var item in result.SearchResults)
                {
                    var icdcodeMatching = dbContext.Icd10Codes.FirstOrDefault(row => row.Code == item.Code);
                    if (icdcodeMatching == null)
                    {
                        icdcodeMatching = new Icd10Code();
                        dbContext.Icd10Codes.Add(icdcodeMatching);
                        icdcodeMatching.ValidFrom =
                            DateTime.Now
                                .AddYears(-30); //initial state - valid code is valid from 30 years in the past as initial value
                    }

                    icdcodeMatching.Code = item.Code;
                    icdcodeMatching.Text = item.Text;
                    icdcodeMatching.LastUpdate = DateTime.Now;
                }

                //revise existing code - if the code was not updated recently (longer than 30 seconds ago) above it must be expired
                var existingItems = dbContext.Icd10Codes.Where(row => row.Code.StartsWith(prefix)).ToList();
                foreach (var existingItem in existingItems)
                {
                    if (existingItem.LastUpdate.HasValue &&
                        existingItem.LastUpdate.Value.Subtract(DateTime.Now).TotalSeconds > 30)
                    {
                        existingItem.ValidTo = DateTime.Now;
                        existingItem.LastUpdate = DateTime.Now;
                    }
                }

                dbContext.SaveChanges();
            }

            reply = "ICD10 codes updated at: " + DateTime.Now;
            return reply;
        }


        [HttpPost]
        public PartialViewResult SearchIcd10(string code)
        {
            var result = GetIcd10Codes(code);
            return PartialView("_SearchResults", result);
        }

        private static SearchResultsDataContract GetIcd10Codes(string code)
        {
            var client =
                new RestClient("https://finnkode.ehelse.no/FinnKodeWS/SearchService.svc/CommitSearch?_dc=1579468909297");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json",
                "{\"searchExpression\":\"" + code +
                "\",\"sources\":[{\"SearchSourceId\":\"ICD10SysDel\",\"Selected\":true}],\"page\":1,\"start\":0,\"limit\":25}",
                ParameterType.RequestBody);
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
                result.SearchResults = result.SearchResults.OrderBy(item => item.Code, new AlphanumericComparator<string>())
                    .ToList();
            }

            return result;
        }
    }
}
