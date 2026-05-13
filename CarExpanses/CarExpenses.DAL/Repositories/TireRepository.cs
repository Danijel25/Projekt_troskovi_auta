using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL.Repositories;

public sealed class TireRepository(CarExpesesDbContext dbContext) : ITireRepository
{
    public IReadOnlyList<Tire> GetAll() => dbContext.Tires
        .Include(tire => tire.CarTires)
        .AsNoTracking()
        .OrderBy(tire => tire.Id)
        .ToList();

    public Tire? GetById(int id) => dbContext.Tires
        .Include(tire => tire.CarTires)
        .AsNoTracking()
        .FirstOrDefault(tire => tire.Id == id);

    public void Add(Tire tire)
    {
        dbContext.Tires.Add(tire);
        dbContext.SaveChanges();
    }

    public bool Update(Tire tire)
    {
        var existing = dbContext.Tires.FirstOrDefault(item => item.Id == tire.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Brand = tire.Brand;
        existing.Model = tire.Model;
        existing.Season = tire.Season;
        existing.Price = tire.Price;

        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var tire = dbContext.Tires
            .Include(item => item.CarTires)
            .FirstOrDefault(item => item.Id == id);

        if (tire is null)
        {
            return false;
        }

        dbContext.Tires.Remove(tire);
        dbContext.SaveChanges();
        return true;
    }
}