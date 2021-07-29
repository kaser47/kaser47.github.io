namespace RecentlyAddedShows.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCreatedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shows", "Created", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shows", "Created");
        }
    }
}
