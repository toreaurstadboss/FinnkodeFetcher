using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinnkodeFetcher.Models
{
    public class SearchResultsDataContract
    {
        public SearchResultsDataContract()
        {
            SearchResults = new List<SearchResultDataContract>();
                
        }
        public List<SearchResultDataContract> SearchResults { get; set; }

        [MinLength(1)]
        [Display(Name="Kode")]
        public string Code { get; set; }
    }
}