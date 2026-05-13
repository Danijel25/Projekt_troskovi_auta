using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers;

[Route("gume")]
public class TiresController(ITireRepository repository, CarExpesesDbContext dbContext) : Controller
{
    [Route("[action]")]
    public IActionResult Index() => View(repository.GetAll());

    [Route("detalji/{id}")]
    public IActionResult Details(int id)
    {
        var tire = repository.GetById(id);
        return tire is null ? NotFound() : View(tire);
    }

    [HttpGet]
    [Route("novi")]
    public IActionResult Create() => View("Form", new TireFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("novi")]
    public IActionResult Create(TireFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", formModel);
        }

        dbContext.Tires.Add(new Tire
        {
            Brand = formModel.Brand,
            Model = formModel.Model,
            Season = formModel.Season,
            Price = formModel.Price
        });

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("uredi/{id}")]
    public IActionResult Edit(int id)
    {
        var tire = dbContext.Tires.AsNoTracking().FirstOrDefault(tire => tire.Id == id);
        return tire is null ? NotFound() : View("Form", new TireFormViewModel
        {
            Id = tire.Id,
            Brand = tire.Brand,
            Model = tire.Model,
            Season = tire.Season,
            Price = tire.Price
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("uredi/{id}")]
    public IActionResult Edit(int id, TireFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", formModel);
        }

        var tire = dbContext.Tires.FirstOrDefault(tire => tire.Id == id);
        if (tire is null)
        {
            return NotFound();
        }

        tire.Brand = formModel.Brand;
        tire.Model = formModel.Model;
        tire.Season = formModel.Season;
        tire.Price = formModel.Price;

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("obrisi/{id}")]
    public IActionResult Delete(int id)
    {
        var tire = dbContext.Tires
            .Include(tire => tire.CarTires)
            .FirstOrDefault(tire => tire.Id == id);

        return tire is null ? NotFound() : View(tire);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("obrisi/{id}")]
    public IActionResult DeleteConfirmed(int id)
    {
        var tire = dbContext.Tires
            .Include(tire => tire.CarTires)
            .FirstOrDefault(tire => tire.Id == id);

        if (tire is null)
        {
            return NotFound();
        }

        dbContext.Tires.Remove(tire);
        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}


