using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace antons_auto.mvc.ViewModels
{
    public class CarViewModel
    {
        [Key]
        public int CarID { get; set; }

        public int CarBrandID { get; set; }

        public string CarModelID { get; set; }

        [DisplayName("Bilmærke")]
        public string CarBrandName { get; set; }

        [DisplayName("Model")]
        public string CarModelName { get; set; }

        [Required(ErrorMessage = "Feltet er påkrævet")]
        [Range(1900, 2050, ErrorMessage = "Årgang skal være mellem 1900 og 2050")]
        [DisplayName("Årgang")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Feltet er påkrævet")]
        [Range(0, 9999999, ErrorMessage = "Prisen skal være mellem 0 og 9.999.999")]
        [DisplayName("Pris")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Feltet er påkrævet")]
        [Range(0, 9999999, ErrorMessage = "Antal Km skal være mellem 0 og 9.999.999")]
        [DisplayName("Km")]
        public int MileAge { get; set; }

        [DisplayName("Billede")]
        public string ImageUrl { get; set; }
    }
}