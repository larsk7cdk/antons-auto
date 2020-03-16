using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using antons_auto.mvc.Data;
using antons_auto.mvc.Data.Entities;
using antons_auto.mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace antons_auto.mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private static string _NO_IMAGE;


        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _NO_IMAGE = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}/images/image_not_available.jpg";

            var carsViewModel = await _context.Cars
                .Include(x => x.CarModel)
                .Include(x => x.CarModel.CarBrand)
                .OrderBy(o => o.CarModel.CarBrand.Name)
                .Take(3)
                .AsNoTracking()
                .Select(car => MapToViewModel(car))
                .ToListAsync();

            return View(carsViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static CarViewModel MapToViewModel(Car car) => new CarViewModel
        {
            CarID = car.CarID,
            CarBrandID = car.CarModel.CarBrandID,
            CarModelID = car.CarModelID.ToString(),
            CarBrandName = car.CarModel.CarBrand.Name,
            CarModelName = car.CarModel.Name,
            Year = car.Year,
            Price = car.Price,
            MileAge = car.MileAge,
            ImageUrl = car.ImageUrl ?? _NO_IMAGE
        };

    }
}