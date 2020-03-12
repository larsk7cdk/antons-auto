using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace antons_auto.mvc.ViewModels
{
    public class CarBrandViewModel
    {
        [Key]
        public int CarBrandID { get; set; }

        [Required(ErrorMessage = "Feltet er påkrævet")]
        [DisplayName("Bilmærke")]
        public string Name { get; set; }
    }
}