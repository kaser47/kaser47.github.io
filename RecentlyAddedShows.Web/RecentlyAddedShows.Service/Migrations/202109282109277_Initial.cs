namespace RecentlyAddedShows.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shows",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.String(),
                        Url = c.String(),
                        Image = c.String(),
                        Created = c.DateTime(nullable: false),
                        NumberViewing = c.Int(nullable: false),
                        IsUpdated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Shows");
        }
    }
}
