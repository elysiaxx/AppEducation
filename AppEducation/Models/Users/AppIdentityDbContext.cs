
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using AppEducation.Models.RoomInfo;
namespace AppEducation.Models.Users{

    public class AppIdentityDbContext : IdentityDbContext<AppUser> {
        
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) {

        }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Document> Documents { get; set; }
        public DbSet<RoomMember> RoomMembers { get; set; }
        public DbSet<RoomDocument> RoomDocuments { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Classes> Classes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Classes>()
                .HasKey(t => t.ClassID);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserProfile>().ToTable("UserProfiles");
            modelBuilder.Entity<UserProfile>().HasKey( t => t.UserProfileId);
          
            modelBuilder.Entity<AppUser>()
                .HasOne( u => u.Profile)
                .WithOne( p => p.User)
                .HasForeignKey<UserProfile>( p => p.UserId);
            modelBuilder.Entity<Document>().HasKey(t => t.DocumentID);
            modelBuilder.Entity<RoomMember>().HasKey(t => t.RoomMemberID);
            modelBuilder.Entity<RoomDocument>().HasKey(t => t.RoomDocumentID);
            modelBuilder.Entity<File>().HasKey(t => t.FileID);
        }

    }
}