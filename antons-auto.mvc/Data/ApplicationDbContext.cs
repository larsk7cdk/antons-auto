using System.Linq;
using antons_auto.mvc.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace antons_auto.mvc.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<CarBrand> CarBrands { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<Car> Cars { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();

            modelBuilder.Entity<CarBrand>().ToTable("CarBrand");
            modelBuilder.Entity<CarModel>().ToTable("CarModel");
            modelBuilder.Entity<Car>().ToTable("Car");
            modelBuilder.Entity<Car>(entity =>
                {
                    entity.Property(e => e.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                }
            );

            
            base.OnModelCreating(modelBuilder);
        }
    }
}