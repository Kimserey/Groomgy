namespace Kimserey.Rating.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsDeactivated_In_UserEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserEntity", "IsDeactivated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserEntity", "IsDeactivated");
        }
    }
}
