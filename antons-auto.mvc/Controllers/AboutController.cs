using System.Linq;
using antons_auto.mvc.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace antons_auto.mvc.Controllers
{
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var imageUrls = _context.Cars
                .Include(x => x.CarModel)
                .Include(x => x.CarModel.CarBrand)
                .Where(s => !string.IsNullOrEmpty(s.ImageUrl))
                .Select(s => s.ImageUrl).ToList();
            
            return View(imageUrls);
        }
    }
}