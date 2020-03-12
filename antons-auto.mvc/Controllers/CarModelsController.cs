﻿using System.Linq;
using System.Threading.Tasks;
using antons_auto.mvc.Data;
using antons_auto.mvc.Data.Entities;
using antons_auto.mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace antons_auto.mvc.Controllers
{
    public class CarModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var carModelsViewModel = await _context.CarModels
                .Include(x => x.CarBrand)
                .Select(carModel => MapToViewModel(carModel))
                .ToListAsync();

            return View(carModelsViewModel.OrderBy(o => o.CarBrandName));
        }

        public async Task<IActionResult> Details(int? id)
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

            var carModel = await _context.CarModels.FindAsync(id);
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
    }
}