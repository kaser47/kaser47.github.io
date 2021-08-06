namespace RecentlyAddedShows.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shows", "IsUpdated", c => c.Boolean(nullable: false));
            AddColumn("dbo.Shows", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shows", "Discriminator");
            DropColumn("dbo.Shows", "IsUpdated");
        }
    }
}
