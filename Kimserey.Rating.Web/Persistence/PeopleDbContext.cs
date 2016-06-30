using Kimserey.Rating.Web.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Persistence
{
    public class PeopleDbContext : DbContext
    {
        public PeopleDbContext() : base("PeopleDbContext")
        { }

        public DbSet<UserEntity> UserEntities { get; set; }
        public DbSet<VoteEntity> VoteEntities { get; set; }
        public DbSet<ConversationEntity> ConversationEntities { get; set; }
        public DbSet<OnlineUserEntity> OnlineUserEntities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<UserEntity>()
                .HasMany(u => u.SentVotes)
                .WithOptional()
                .HasForeignKey(u => u.RatedByUserId);

            modelBuilder.Entity<UserEntity>()
                .HasMany(u => u.ReceivedVotes)
                .WithOptional()
                .HasForeignKey(u => u.RatedOnUserId);

            modelBuilder.Entity<VoteEntity>()
                .HasRequired(v => v.RatedByUser)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VoteEntity>()
                .HasRequired(v => v.RatedOnUser)
                .WithMany()
                .WillCascadeOnDelete(false);
        }
    }
}
