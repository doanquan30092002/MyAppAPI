using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;

namespace MyApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions)
            : base(dbContextOptions) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa chính của bảng UserRole
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

            // Cấu hình quan hệ giữa User và UserRole
            modelBuilder
                .Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            // Cấu hình quan hệ giữa Role và UserRole
            modelBuilder
                .Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
            modelBuilder
                .Entity<Role>()
                .HasData(
                    new Role { Id = "2282fcab-6ead-463b-86d1-fee0b313ab0d", RoleName = "Admin" },
                    new Role { Id = "61d6e025-bbd2-429d-922f-e02c8be9d866", RoleName = "User" }
                );
        }
    }
}
