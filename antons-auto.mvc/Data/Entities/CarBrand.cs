using System.ComponentModel.DataAnnotations;

namespace antons_auto.mvc.Data.Entities
{
    public class CarBrand
    {
        public int CarBrandID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}