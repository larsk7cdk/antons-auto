using System.Linq;
using System.Threading.Tasks;
using antons_auto.mvc.Data;
using antons_auto.mvc.Data.Entities;
using antons_auto.mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace antons_auto.mvc.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var carsViewModel = await _context.Cars
                .Include(x => x.CarModel)
                .Include(x => x.CarModel.CarBrand)
                .OrderBy(o => o.CarModel.CarBrand.Name)
                .AsNoTracking()
                .Select(car => MapToViewModel(car))
                .ToListAsync();

            return View(carsViewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var car = await _context.Cars
                .Include(x => x.CarModel)
                .Include(x => x.CarModel.CarBrand)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CarID == id);

            if (car == null) return NotFound();
            var carViewModel = MapToViewModel(car);

            return View(carViewModel);
        }

        public async Task<IActionResult> Create()
        {
            await ViewDataCarBrands();
            await ViewDataCarModels();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CarId,CarBrandID,CarModelID,Year,Price,MileAge,ImageUrl")]
            CarViewModel carViewModel)
        {
            if (ModelState.IsValid)
            {
                var car = MapToModel(carViewModel);
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(carViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var car = await _context.Cars
                .Include(x => x.CarModel)
                .Include(x => x.CarModel.CarBrand)
                .FirstOrDefaultAsync(m => m.CarID == id);

            if (car == null) return NotFound();
            var carViewModel = MapToViewModel(car);

            return View(carViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarID,CarBrandID, CarModelID,Year,Price,MileAge,ImageUrl")]
            CarViewModel carViewModel)
        {
            if (id != carViewModel.CarID) return NotFound();

            if (ModelState.IsValid)
            {
                var car = MapToModel(carViewModel);
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.CarID))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(carViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var car = await _context.Cars
                .Include(x => x.CarModel)
                .Include(x => x.CarModel.CarBrand)
                .FirstOrDefaultAsync(m => m.CarID == id);

            if (car == null) return NotFound();
            var carViewModel = MapToViewModel(car);

            return View(carViewModel);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // Private methods
        private bool CarExists(int id) => _context.Cars.Any(e => e.CarID == id);

        private async Task ViewDataCarBrands() =>
            ViewData["CarBrands"] = new SelectList(await _context.CarBrands
                .OrderBy(o => o.Name)
                .AsNoTracking()
                .ToListAsync(), "CarBrandID", "Name", "CarBrandID");

        private async Task ViewDataCarModels() =>
        ViewData["CarModels"] = new SelectList(await _context.CarModels
            .OrderBy(o => o.Name)
            .Select(s => new { Id = $"{s.CarBrandID}#{s.CarModelID}", s.Name })
            .AsNoTracking()
            .ToListAsync(), "Id", "Name");

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
            ImageUrl = car.ImageUrl
        };


        private static Car MapToModel(CarViewModel carViewModel)
        {
            var carModelID = int.Parse(carViewModel.CarModelID.Contains("#")
                ? carViewModel.CarModelID.Split("#")[1]
                : carViewModel.CarModelID);

            return new Car
            {
                CarID = carViewModel.CarID,
                CarModelID = carModelID,
                Year = carViewModel.Year,
                Price = carViewModel.Price,
                MileAge = carViewModel.MileAge,
                ImageUrl = carViewModel.ImageUrl
            };
        }
    }
}