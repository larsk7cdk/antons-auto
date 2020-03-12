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
        [DisplayName("Årstal")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Feltet er påkrævet")]
        [DisplayName("Pris")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Feltet er påkrævet")]
        [DisplayName("Km")]
        public int MileAge { get; set; }

        [DisplayName("Billede")]
        public string ImageUrl { get; set; }


    }
}