using FinnkodeFetcher.DbContext.Models;
using System.Data.Entity;

namespace FinnkodeFetcher.DbContext
{
    public class FinnkodeFetcherDbContext : System.Data.Entity.DbContext
    {

        public FinnkodeFetcherDbContext() : base("DefaultConnection")
        {
                
        }

        public DbSet<Icd10Code> Icd10Codes { get; set; }
        public DbSet<NcspCode> NcspCodes { get; set; }
    }
}