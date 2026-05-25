using CarExpenses.DAL.Repositories;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarExpenses.Web.Controllers
{
    public class HomeController(ICarRepository carRepository) : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            var cars = carRepository
                .GetAll()
                .OrderByDescending(car => car.Expenses?.Sum(expense => expense.Amount) ?? 0m)
                .ToList();

            return View(cars);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


