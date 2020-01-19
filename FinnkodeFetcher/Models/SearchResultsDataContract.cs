﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        public string Code { get; set; }
    }
}