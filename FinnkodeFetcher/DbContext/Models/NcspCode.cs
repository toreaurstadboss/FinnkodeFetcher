using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinnkodeFetcher.DbContext.Models
{
 
    [Table("NcspCodes")]
    public class NcspCode
    {
        [Key] 
        public int Id { get; set; }

        public string Code { get; set; }

        public string Text { get; set; }

        public string Catalog { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}