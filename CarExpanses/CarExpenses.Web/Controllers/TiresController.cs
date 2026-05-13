using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

[Route("gume")]
public class TiresController(ITireRepository repository) : Controller
{
    [Route("[action]")]
    public IActionResult Index() => View(repository.GetAll());

    [HttpGet]
    [Route("pretraga")]
    public IActionResult Search(string? query)
    {
        var tires = repository.GetAll();

        if (string.IsNullOrWhiteSpace(query))
        {
            return PartialView("_TireList", tires);
        }

        var term = query.Trim();
        var filtered = tires
            .Where(tire =>
                tire.Brand.Contains(term, StringComparison.OrdinalIgnoreCase)
                || tire.Model.Contains(term, StringComparison.OrdinalIgnoreCase)
                || tire.Season.Contains(term, StringComparison.OrdinalIgnoreCase)
                || tire.Price.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || tire.Id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return PartialView("_TireList", filtered);
    }

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

        repository.Add(new Tire
        {
            Brand = formModel.Brand,
            Model = formModel.Model,
            Season = formModel.Season,
            Price = formModel.Price
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("uredi/{id}")]
    public IActionResult Edit(int id)
    {
        var tire = repository.GetById(id);
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

        var tire = new Tire
        {
            Id = formModel.Id,
            Brand = formModel.Brand,
            Model = formModel.Model,
            Season = formModel.Season,
            Price = formModel.Price
        };

        if (!repository.Update(tire))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("obrisi/{id}")]
    public IActionResult Delete(int id)
    {
        var tire = repository.GetById(id);
        return tire is null ? NotFound() : View(tire);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("obrisi/{id}")]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!repository.Delete(id))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }
}


