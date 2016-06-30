namespace Kimserey.Rating.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addshowWelcometouser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserEntity", "ShowWelcome", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserEntity", "ShowWelcome");
        }
    }
}
