using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace antons_auto.mvc.ViewModels
{
    public class CarModelViewModel
    {
        [Key]
        public int CarModelID { get; set; }

        public int CarBrandID { get; set; }

        [DisplayName("Bilmærke")]
        public string CarBrandName { get; set; }

        [Required(ErrorMessage = "Feltet er påkrævet")]
        [DisplayName("Model")]
        public string CarModelName { get; set; }
    }
}