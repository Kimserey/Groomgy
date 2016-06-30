namespace Kimserey.Rating.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmailNotificationEnabled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserEntity", "EmailNotificationEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserEntity", "EmailNotificationEnabled");
        }
    }
}
