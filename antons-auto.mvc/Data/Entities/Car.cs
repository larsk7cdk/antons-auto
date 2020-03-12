using System;
using System.ComponentModel.DataAnnotations;

namespace antons_auto.mvc.Data.Entities
{
    public class Car
    {
        public int CarID { get; set; }
        public int CarModelID { get; set; }

        public DateTime CreationDate { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int MileAge { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; }

        public CarModel CarModel { get; set; }
    }
}