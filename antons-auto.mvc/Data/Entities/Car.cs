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

        [Required]
        public string Address { get; set; }

        [Required]
        public string AddressNo { get; set; }

        [Required]
        public int PostalCode { get; set; }

        public string City { get; set; }

        public float Longitude { get; set; }

        public float Latitude { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; }

        public CarModel CarModel { get; set; }
    }
}