using System.ComponentModel.DataAnnotations;

namespace antons_auto.mvc.Data.Entities
{
    public class CarModel
    {
        public int CarModelID { get; set; }
        public int CarBrandID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public CarBrand CarBrand { get; set; }
    }
}