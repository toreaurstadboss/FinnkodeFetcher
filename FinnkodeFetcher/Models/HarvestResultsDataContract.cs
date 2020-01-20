using System.ComponentModel.DataAnnotations;

namespace FinnkodeFetcher.Models
{
    public class HarvestResultsDataContract
    {
        [Display(Name="Prefiks fra")]
        public string PrefixFrom { get; set; }
        [Display(Name="Prefiks til")]
        public string PrefixTo { get; set; }
        public string HarvestResults { get; set; }
    }
}