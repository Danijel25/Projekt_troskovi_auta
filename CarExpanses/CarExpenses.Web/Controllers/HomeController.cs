using CarExpenses.DAL.Repositories;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarExpenses.Web.Controllers
{
    public class HomeController(ICarRepository carRepository) : Controller
    {
        public IActionResult Index()
        {
            var cars = carRepository
                .GetAll()
                .OrderByDescending(car => car.Expenses?.Sum(expense => expense.Amount) ?? 0m)
                .ToList();

            return View(cars);
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
    }
}


