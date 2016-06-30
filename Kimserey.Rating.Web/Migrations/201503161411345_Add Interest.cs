namespace Kimserey.Rating.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInterest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserEntity", "Interest", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserEntity", "Interest");
        }
    }
}
