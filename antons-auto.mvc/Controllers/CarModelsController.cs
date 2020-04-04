using System;
using System.Linq;
using System.Threading.Tasks;
using antons_auto.mvc.Data;
using antons_auto.mvc.Data.Entities;
using antons_auto.mvc.Shared;
using antons_auto.mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace antons_auto.mvc.Controllers
{
    public class CarModelsController : Controller
    {
        private const int PAGE_SIZE = 3;
        private readonly ApplicationDbContext _context;

        public CarModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder = "", int pageNumber = 1)
        {
            var carModels = _context.CarModels
                .Include(x => x.CarBrand);

            var carModelsSorted = SortCarModels(carModels, sortOrder).AsNoTracking();
            var paginatedList = await PaginatedList<CarModel>.CreateAsync(carModelsSorted, pageNumber, PAGE_SIZE);
            var carModelsViewModel = paginatedList.Select(MapToViewModel);

            ViewData["NameSortParm"] = sortOrder;
            ViewData["pages"] = (int)(Math.Ceiling((decimal)carModelsSorted.Count() / (decimal)PAGE_SIZE));
            ViewData["pageIndex"] = pageNumber;

            return View(carModelsViewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var carModel = await _context.CarModels
                .Include(x => x.CarBrand)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CarModelID == id);
            if (carModel == null) return NotFound();

            await ViewDataCarBrands();
            var carModelViewModel = MapToViewModel(carModel);

            return View(carModelViewModel);
        }

        public async Task<IActionResult> Create()
        {
            await ViewDataCarBrands();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CarModelID,CarBrandID,CarModelName")]
            CarModelViewModel carModelViewModel)
        {
            if (ModelState.IsValid)
            {
                var carModel = MapToModel(carModelViewModel);
                _context.Add(carModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(carModelViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var carModel = await _context.CarModels
                  .Include(x => x.CarBrand)
                  .FirstOrDefaultAsync(m => m.CarModelID == id);
            if (carModel == null) return NotFound();

            await ViewDataCarBrands();
            var carModelViewModel = MapToViewModel(carModel);

            return View(carModelViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarModelID,CarBrandID,CarModelName")]
            CarModelViewModel carModelViewModel)
        {
            if (id != carModelViewModel.CarModelID) return NotFound();

            if (ModelState.IsValid)
            {
                var carModel = MapToModel(carModelViewModel);
                try
                {
                    _context.Update(carModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarModelExists(carModel.CarModelID))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(carModelViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var carModel = await _context.CarModels
                .Include(x => x.CarBrand)
                .FirstOrDefaultAsync(m => m.CarModelID == id);

            if (carModel == null) return NotFound();
            var carModelViewModel = MapToViewModel(carModel);

            return View(carModelViewModel);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carModel = await _context.CarModels.FindAsync(id);
            _context.CarModels.Remove(carModel);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // Private methods
        private bool CarModelExists(int id) => _context.CarModels.Any(e => e.CarModelID == id);

        private async Task ViewDataCarBrands() =>
            ViewData["CarBrands"] = new SelectList(await _context.CarBrands
                .OrderBy(o => o.Name)
                .AsNoTracking()
                .ToListAsync(), "CarBrandID", "Name", "CarBrandID");


        private static CarModelViewModel MapToViewModel(CarModel carModel) => new CarModelViewModel
        {
            CarModelID = carModel.CarModelID,
            CarBrandID = carModel.CarBrandID,
            CarModelName = carModel.Name,
            CarBrandName = carModel.CarBrand.Name
        };

        private static CarModel MapToModel(CarModelViewModel carModelViewModel) => new CarModel
        {
            CarModelID = carModelViewModel.CarModelID,
            CarBrandID = carModelViewModel.CarBrandID,
            Name = carModelViewModel.CarModelName
        };

        private static IQueryable<CarModel> SortCarModels(IQueryable<CarModel> model, string sortOrder)
        {
            return sortOrder switch
            {
                "name_desc" => model.OrderByDescending(o => o.CarBrand.Name),
                _ => model.OrderBy(o => o.CarBrand.Name)
            };
        }
    }
}