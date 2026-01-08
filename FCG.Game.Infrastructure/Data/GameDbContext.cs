using FCG.Game.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Game.Infrastructure.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }

        public DbSet<FCG.Game.Domain.Entities.Game> Games { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<UserLibraryGame> UserLibraryGames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Game entity
            modelBuilder.Entity<FCG.Game.Domain.Entities.Game>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Genre).HasMaxLength(100);
                entity.Property(e => e.Tags)
                    .HasMaxLength(1000)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

            });

            // Configure OrderItem entity
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GameId).IsRequired();
                entity.Property(e => e.GameTitle).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Quantity).IsRequired();
            });

            modelBuilder.Entity<UserLibraryGame>(entity =>
            {
                entity.HasKey(e => e.idLibraryGame);
                entity.Property(e => e.orderId).IsRequired();
                entity.Property(e => e.userId).IsRequired();
                entity.Property(e => e.idGame).IsRequired();
                entity.Property(e => e.isActive).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.createdAt).IsRequired().HasDefaultValue(DateTime.UtcNow);
            });
        }
    }
}
