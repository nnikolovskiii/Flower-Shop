using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Test1.Models;
using Test1.Models.Relations;

namespace Test1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Test1.Models.FlowerType>? FlowerType { get; set; }
        public DbSet<Test1.Models.Occasion>? Occasion { get; set; }

        public DbSet<Test1.Models.FlowerItem>? FlowerItems { get; set; }
        public DbSet<Test1.Models.Order>? Orders { get; set; }
        public DbSet<FlowerItemOccasion>? FlowerItemOccasions { get; set; }

        public DbSet<FlowerItemFlowerType>? FlowerItemFlowerTypes { get; set; }

        public DbSet<FlowerItemOrder>? FlowerItemOrder { get; set; }
        public DbSet<ColorItem> ColorItems { get; set; }

        public DbSet<SizeItem> SizeItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            base.OnModelCreating(modelBuilder);
            /*// One-to-Many Relationship
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);*/

            // Many-to-Many Relationship
            modelBuilder.Entity<FlowerItemOccasion>()
                .HasKey(fio => new { fio.FlowerItemId, fio.OccasionId });

            modelBuilder.Entity<FlowerItemOccasion>()
                .HasOne(fio => fio.FlowerItem)
                .WithMany(fi => fi.Occasions)
                .HasForeignKey(fio => fio.FlowerItemId);

            modelBuilder.Entity<FlowerItemOccasion>()
                .HasOne(fio => fio.Occasion)
                .WithMany(o => o.FlowerItems)
                .HasForeignKey(fio => fio.OccasionId);


            modelBuilder.Entity<FlowerItemFlowerType>()
                .HasKey(fio => new { fio.FlowerItemId, fio.FlowerTypeId });

            modelBuilder.Entity<FlowerItemFlowerType>()
                .HasOne(fio => fio.FlowerItem)
                .WithMany(fi => fi.FlowerTypes)
                .HasForeignKey(fio => fio.FlowerItemId);

            modelBuilder.Entity<FlowerItemFlowerType>()
                .HasOne(fio => fio.FlowerType)
                .WithMany(o => o.FlowerItems)
                .HasForeignKey(fio => fio.FlowerTypeId);


            modelBuilder.Entity<FlowerItem>()
                .HasMany(fi => fi.ColorItems)
                .WithOne(ci => ci.FlowerItem)
                .HasForeignKey(ci => ci.FlowerItemId);

            modelBuilder.Entity<FlowerItem>()
                .HasMany(fi => fi.SizeItems)
                .WithOne(ci => ci.FlowerItem)
                .HasForeignKey(ci => ci.FlowerItemId);

            modelBuilder.Entity<FlowerItemOrder>()
                .HasKey(fio => new { fio.FlowerItemId, fio.OrderId });

            modelBuilder.Entity<FlowerItemOrder>()
                .HasOne(fio => fio.FlowerItem)
                .WithMany(fi => fi.Orders)
                .HasForeignKey(fio => fio.FlowerItemId);

            modelBuilder.Entity<FlowerItemOrder>()
                .HasOne(fio => fio.Order)
                .WithMany(o => o.FlowerItems)
                .HasForeignKey(fio => fio.OrderId);
        }

        public IEnumerable<FlowerItem> GetSizeItems(int limit, int offset)
        {
            var sql = "SELECT * FROM FlowerItems LIMIT @limit OFFSET @offset;";
            var parameters = new[]
            {
        new MySqlParameter("@limit", limit),
        new MySqlParameter("@offset", offset)
    };

            return this.Set<FlowerItem>().FromSqlRaw(sql, parameters).ToList();
        }


    }
}