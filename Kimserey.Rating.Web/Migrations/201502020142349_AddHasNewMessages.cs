namespace Kimserey.Rating.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHasNewMessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConversationOptionEntity", "HasNewMessages", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConversationOptionEntity", "HasNewMessages");
        }
    }
}
