using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers;

public class UsersController(IUserRepository repository, CarExpesesDbContext dbContext) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var user = repository.GetById(id);
        return user is null ? NotFound() : View(user);
    }
    [HttpGet]
    public IActionResult Create() => View("Form", new UserFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UserFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", formModel);
        }

        dbContext.Users.Add(new User
        {
            Username = formModel.Username,
            Email = formModel.Email,
            Password = formModel.Password
        });

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var user = dbContext.Users.AsNoTracking().FirstOrDefault(user => user.Id == id);
        return user is null ? NotFound() : View("Form", new UserFormViewModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Password = user.Password
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, UserFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", formModel);
        }

        var user = dbContext.Users.FirstOrDefault(user => user.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        user.Username = formModel.Username;
        user.Email = formModel.Email;
        user.Password = formModel.Password;

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var user = dbContext.Users
            .Include(user => user.Cars)!
                .ThenInclude(car => car.FuelExpenses)
            .Include(user => user.Cars)!
                .ThenInclude(car => car.ServiceRecords)
            .Include(user => user.Cars)!
                .ThenInclude(car => car.Insurances)
            .Include(user => user.Cars)!
                .ThenInclude(car => car.CarTires)!
                    .ThenInclude(carTire => carTire.Tire)
            .Include(user => user.Cars)!
                .ThenInclude(car => car.Expenses)
            .FirstOrDefault(user => user.Id == id);

        return user is null ? NotFound() : View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var user = dbContext.Users
            .Include(user => user.Cars)!
                .ThenInclude(car => car.FuelExpenses)
            .Include(user => user.Cars)!
                .ThenInclude(car => car.ServiceRecords)
            .Include(user => user.Cars)!
                .ThenInclude(car => car.Insurances)
            .Include(user => user.Cars)!
                .ThenInclude(car => car.CarTires)!
                    .ThenInclude(carTire => carTire.Tire)
            .Include(user => user.Cars)!
                .ThenInclude(car => car.Expenses)
            .FirstOrDefault(user => user.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        dbContext.Users.Remove(user);
        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

}


