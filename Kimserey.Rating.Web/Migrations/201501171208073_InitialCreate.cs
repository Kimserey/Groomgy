namespace Kimserey.Rating.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConversationEntity",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConversationOptionEntity",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UserId = c.Guid(nullable: false),
                        ConversationEntity_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ConversationEntity", t => t.ConversationEntity_Id)
                .Index(t => t.ConversationEntity_Id);
            
            CreateTable(
                "dbo.MessageEntity",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Text = c.String(),
                        SentOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        SentByUserId = c.Guid(nullable: false),
                        ConversationId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ConversationEntity", t => t.ConversationId, cascadeDelete: true)
                .ForeignKey("dbo.UserEntity", t => t.SentByUserId, cascadeDelete: true)
                .Index(t => t.SentByUserId)
                .Index(t => t.ConversationId);
            
            CreateTable(
                "dbo.UserEntity",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Email = c.String(),
                        Birthday = c.DateTime(nullable: false),
                        Description = c.String(),
                        ProfilePhotoUrl = c.String(),
                        Gender = c.Int(nullable: false),
                        Location = c.String(),
                        AlbumPhotoUrls = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VoteEntity",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Rate = c.Int(nullable: false),
                        RatedOn = c.DateTime(nullable: false),
                        Comment = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        RatedByUserId = c.Guid(nullable: false),
                        RatedOnUserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserEntity", t => t.RatedByUserId)
                .ForeignKey("dbo.UserEntity", t => t.RatedOnUserId)
                .Index(t => t.RatedByUserId)
                .Index(t => t.RatedOnUserId);
            
            CreateTable(
                "dbo.OnlineUserEntity",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SignalRConnectionId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserEntityConversationEntity",
                c => new
                    {
                        UserEntity_Id = c.Guid(nullable: false),
                        ConversationEntity_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserEntity_Id, t.ConversationEntity_Id })
                .ForeignKey("dbo.UserEntity", t => t.UserEntity_Id, cascadeDelete: true)
                .ForeignKey("dbo.ConversationEntity", t => t.ConversationEntity_Id, cascadeDelete: true)
                .Index(t => t.UserEntity_Id)
                .Index(t => t.ConversationEntity_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessageEntity", "SentByUserId", "dbo.UserEntity");
            DropForeignKey("dbo.VoteEntity", "RatedOnUserId", "dbo.UserEntity");
            DropForeignKey("dbo.VoteEntity", "RatedByUserId", "dbo.UserEntity");
            DropForeignKey("dbo.UserEntityConversationEntity", "ConversationEntity_Id", "dbo.ConversationEntity");
            DropForeignKey("dbo.UserEntityConversationEntity", "UserEntity_Id", "dbo.UserEntity");
            DropForeignKey("dbo.MessageEntity", "ConversationId", "dbo.ConversationEntity");
            DropForeignKey("dbo.ConversationOptionEntity", "ConversationEntity_Id", "dbo.ConversationEntity");
            DropIndex("dbo.UserEntityConversationEntity", new[] { "ConversationEntity_Id" });
            DropIndex("dbo.UserEntityConversationEntity", new[] { "UserEntity_Id" });
            DropIndex("dbo.VoteEntity", new[] { "RatedOnUserId" });
            DropIndex("dbo.VoteEntity", new[] { "RatedByUserId" });
            DropIndex("dbo.MessageEntity", new[] { "ConversationId" });
            DropIndex("dbo.MessageEntity", new[] { "SentByUserId" });
            DropIndex("dbo.ConversationOptionEntity", new[] { "ConversationEntity_Id" });
            DropTable("dbo.UserEntityConversationEntity");
            DropTable("dbo.OnlineUserEntity");
            DropTable("dbo.VoteEntity");
            DropTable("dbo.UserEntity");
            DropTable("dbo.MessageEntity");
            DropTable("dbo.ConversationOptionEntity");
            DropTable("dbo.ConversationEntity");
        }
    }
}
