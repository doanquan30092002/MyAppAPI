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
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<AuctionAssets> AuctionAssets { get; set; }
        public DbSet<AuctionDocuments> AuctionDocuments { get; set; }
        public DbSet<AuctionCategory> AuctionCategories { get; set; }
        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Blacklist> Blacklists { get; set; }
        public DbSet<Information> Informations { get; set; }
        public DbSet<BlogType> BlogTypes { get; set; }
        public DbSet<AuctionRound> AuctionRounds { get; set; }
        public DbSet<AuctionRoundPrices> AuctionRoundPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Auction>()
                .HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
