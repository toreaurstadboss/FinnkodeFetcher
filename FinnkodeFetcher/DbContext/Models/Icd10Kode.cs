using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinnkodeFetcher.DbContext.Models
{
    [Table("Icd10Codes")]
    public class Icd10Code
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Text { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}