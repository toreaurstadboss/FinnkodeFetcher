namespace FinnkodeFetcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSeedDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Icd10Codes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Text = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NcspCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Text = c.String(),
                        Catalog = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NcspCodes");
            DropTable("dbo.Icd10Codes");
        }
    }
}
