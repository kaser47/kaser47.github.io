namespace RecentlyAddedShows.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsUpdated1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Shows", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shows", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
    }
}
