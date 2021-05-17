
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace AppEducation.Models.Users{

    public class AppIdentityDbContext : IdentityDbContext<AppUser> {
        
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) {
        }
       
        public DbSet<UserProfile> UserProfiles {get; set;}

        public DbSet<Classes> Classes { get; set; }
        public DbSet<HistoryOfClass> HOClasses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.Entity<IdentityUser>().ToTable("Users");
            // modelBuilder.Entity<AppUser>( entity => 
            // {
            //     entity.ToTable(name:"Users");
            // });
            // modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            // modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            // modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            // modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<Classes>()
                .HasKey(t => t.ClassID);
            modelBuilder.Entity<HistoryOfClass>()
                .HasKey(t => t.hocID);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserProfile>().ToTable("UserProfiles");
            modelBuilder.Entity<UserProfile>().HasKey( t => t.UserProfileId);
          
            modelBuilder.Entity<AppUser>()
                .HasOne( u => u.Profile)
                .WithOne( p => p.User)
                .HasForeignKey<UserProfile>( p => p.UserId);
        }

    }
}