using System;
using System.Linq;
using System.Threading.Tasks;
using antons_auto.mvc.Data;
using antons_auto.mvc.Data.Entities;
using antons_auto.mvc.Shared;
using antons_auto.mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace antons_auto.mvc.Controllers
{
    public class CarBrandsController : Controller
    {
        private const int PAGE_SIZE = 3;
        private readonly ApplicationDbContext _context;

        public CarBrandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var carBrands =
                 _context.CarBrands
                    .OrderBy(o => o.Name)
                    .AsNoTracking();

            var paginatedList = await PaginatedList<CarBrand>.CreateAsync(carBrands, pageNumber, PAGE_SIZE);
            var carBrandsViewModel = paginatedList.Select(MapToViewModel);

            ViewData["pages"] = (int)(Math.Ceiling((decimal)carBrands.Count() / (decimal)PAGE_SIZE));
            ViewData["pageIndex"] = pageNumber;

            return View(carBrandsViewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var carBrand = await _context.CarBrands
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CarBrandID == id);

            if (carBrand == null) return NotFound();
            var carBrandViewModel = MapToViewModel(carBrand);

            return View(carBrandViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarBrandID,Name")] CarBrandViewModel carBrandViewModel)
        {
            if (ModelState.IsValid)
            {
                var carBrand = MapToModel(carBrandViewModel);
                _context.Add(carBrand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(carBrandViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var carBrand = await _context.CarBrands.FindAsync(id);
            if (carBrand == null) return NotFound();
            var carBrandViewModel = MapToViewModel(carBrand);

            return View(carBrandViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarBrandID,Name")] CarBrandViewModel carBrandViewModel)
        {
            if (id != carBrandViewModel.CarBrandID) return NotFound();

            if (ModelState.IsValid)
            {
                var carBrand = MapToModel(carBrandViewModel);
                try
                {
                    _context.Update(carBrand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarBrandExists(carBrand.CarBrandID))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(carBrandViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var carBrand = await _context.CarBrands
                .FirstOrDefaultAsync(m => m.CarBrandID == id);
            if (carBrand == null) return NotFound();

            var carBrandViewModel = MapToViewModel(carBrand);

            return View(carBrandViewModel);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carBrand = await _context.CarBrands.FindAsync(id);
            _context.CarBrands.Remove(carBrand);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Private methods
        private bool CarBrandExists(int id) => _context.CarBrands.Any(e => e.CarBrandID == id);

        private static CarBrandViewModel MapToViewModel(CarBrand carBrand) => new CarBrandViewModel
        {
            CarBrandID = carBrand.CarBrandID,
            Name = carBrand.Name
        };

        private static CarBrand MapToModel(CarBrandViewModel carBrandViewModel) => new CarBrand
        {
            CarBrandID = carBrandViewModel.CarBrandID,
            Name = carBrandViewModel.Name
        };
    }
}
