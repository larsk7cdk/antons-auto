using Microsoft.EntityFrameworkCore;

namespace antons_auto.mvc.Data.Entities
{
    public static class AppDbSeed
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarBrand>().HasData(
                new CarBrand { CarBrandID = 1, Name = "BMW" },
                new CarBrand { CarBrandID = 2, Name = "VW" },
                new CarBrand { CarBrandID = 3, Name = "Seat" }
            );

            modelBuilder.Entity<CarModel>().HasData(
                new CarModel { CarModelID = 1, CarBrandID = 1, Name = "X1" },
                new CarModel { CarModelID = 2, CarBrandID = 1, Name = "X2" },
                new CarModel { CarModelID = 3, CarBrandID = 1, Name = "X3" },
                new CarModel { CarModelID = 4, CarBrandID = 2, Name = "Golf" },
                new CarModel { CarModelID = 5, CarBrandID = 3, Name = "Arona" }
            );
        }
    }
}