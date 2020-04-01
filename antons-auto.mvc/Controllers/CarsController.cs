using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using antons_auto.mvc.Data;
using antons_auto.mvc.Data.Entities;
using antons_auto.mvc.ServiceProxies;
using antons_auto.mvc.Shared;
using antons_auto.mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace antons_auto.mvc.Controllers
{
    public class CarsController : Controller
    {
        private const int PAGE_SIZE = 6;
        private const string GOOGLE_MAPS_KEY = "AIzaSyArmzkH-4mqeXhynpdKa-1xRdjCTInXRzY";
        private static string _noImage;

        private readonly ApplicationDbContext _context;
        private readonly IDawaServiceProxy _dawaServiceProxy;

        private readonly List<SelectListItem> _sorting = new List<SelectListItem>
        {
            new SelectListItem {Value = "newest", Text = "Nyeste", Selected = false},
            new SelectListItem {Value = "carmodel", Text = "Bilmærke", Selected = false},
            new SelectListItem {Value = "price", Text = "Pris", Selected = false}
        };

        public CarsController(ApplicationDbContext context, IDawaServiceProxy dawaServiceProxy)
        {
            _context = context;
            _dawaServiceProxy = dawaServiceProxy;
        }

        public async Task<IActionResult> Index(
            string sortOrder = "newest",
            string searchString = "",
            int pageNumber = 1)
        {
            _noImage =
                $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}/images/image_not_available.jpg";

            _sorting.ForEach(f => f.Selected = f.Value == sortOrder);


            var cars = _context.Cars
                .Include(x => x.CarModel)
                .Include(x => x.CarModel.CarBrand);

            var carsFiltered = CarsFiltered(searchString, cars);
            var carsSorted = SortCars(carsFiltered, sortOrder).AsNoTracking();
            var paginatedList = await PaginatedList<Car>.CreateAsync(carsSorted, pageNumber, PAGE_SIZE);
            var carsViewModel = paginatedList.Select(MapToViewModel);

            ViewData["sorting"] = _sorting;
            ViewData["sortOrderSelect"] = sortOrder;
            ViewData["currentFilter"] = searchString;
            ViewData["pages"] = (int)(Math.Ceiling((decimal)carsSorted.Count() / (decimal)PAGE_SIZE));
            ViewData["pageIndex"] = pageNumber;

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
            ViewData["google-maps-key"] = GOOGLE_MAPS_KEY;
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
            [Bind("CarId,CarBrandID,CarModelID,Year,Price,MileAge,Address,AddressNo,PostalCode,ImageUrl")]
            CarViewModel carViewModel)
        {
            if (ModelState.IsValid)
            {
                var locationModel = _dawaServiceProxy.GetLocation(carViewModel.Address, carViewModel.AddressNo,
                    carViewModel.PostalCode);
                carViewModel.City = locationModel.City;
                carViewModel.Longitude = locationModel.Longitude;
                carViewModel.Latitude = locationModel.Latitude;

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
            carViewModel.ImageUrl = carViewModel.ImageUrl.Equals(_noImage) ? string.Empty : carViewModel.ImageUrl;

            return View(carViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("CarID,CarBrandID, CarModelID,Year,Price,MileAge,Address,AddressNo,PostalCode,ImageUrl")]
            CarViewModel carViewModel)
        {
            if (id != carViewModel.CarID) return NotFound();

            if (ModelState.IsValid)
            {
                var locationModel = _dawaServiceProxy.GetLocation(carViewModel.Address, carViewModel.AddressNo,
                    carViewModel.PostalCode);
                carViewModel.City = locationModel.City;
                carViewModel.Longitude = locationModel.Longitude;
                carViewModel.Latitude = locationModel.Latitude;

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
        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.CarID == id);
        }

        private async Task ViewDataCarBrands()
        {
            ViewData["CarBrands"] = new SelectList(await _context.CarBrands
                .OrderBy(o => o.Name)
                .AsNoTracking()
                .ToListAsync(), "CarBrandID", "Name", "CarBrandID");
        }

        private async Task ViewDataCarModels()
        {
            ViewData["CarModels"] = new SelectList(await _context.CarModels
                .OrderBy(o => o.Name)
                .Select(s => new { Id = $"{s.CarBrandID}#{s.CarModelID}", s.Name })
                .AsNoTracking()
                .ToListAsync(), "Id", "Name");
        }

        private static IQueryable<Car> CarsFiltered(string searchString, IIncludableQueryable<Car, CarBrand> cars)
        {
            var carsFiltered = cars.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                carsFiltered = cars.Where(s =>
                        s.CarModel.CarBrand.Name.Contains(searchString) ||
                        s.CarModel.Name.Contains(searchString));

            return carsFiltered;
        }

        private static IQueryable<Car> SortCars(IQueryable<Car> cars, string sortOrder)
        {
            return sortOrder switch
            {
                "newest" => cars.OrderBy(o => o.CreationDate),
                "price" => cars.OrderBy(o => o.Price),
                _ => cars.OrderBy(o => o.CarModel.CarBrand.Name)
            };
        }

        private static CarViewModel MapToViewModel(Car car)
        {
            return new CarViewModel
            {
                CarID = car.CarID,
                CarBrandID = car.CarModel.CarBrandID,
                CarModelID = car.CarModelID.ToString(),
                CarBrandName = car.CarModel.CarBrand.Name,
                CarModelName = car.CarModel.Name,
                Year = car.Year,
                Price = car.Price,
                MileAge = car.MileAge,
                Address = car.Address,
                AddressNo = car.AddressNo,
                PostalCode = car.PostalCode,
                City = car.City,
                FullAddress = $"{car.Address} {car.AddressNo}, {car.City}",
                Longitude = car.Longitude,
                Latitude = car.Latitude,
                ImageUrl = car.ImageUrl ?? _noImage
            };
        }


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
                Address = carViewModel.Address,
                AddressNo = carViewModel.AddressNo,
                PostalCode = carViewModel.PostalCode,
                City = carViewModel.City,
                Longitude = carViewModel.Longitude,
                Latitude = carViewModel.Latitude,
                ImageUrl = carViewModel.ImageUrl
            };
        }
    }
}