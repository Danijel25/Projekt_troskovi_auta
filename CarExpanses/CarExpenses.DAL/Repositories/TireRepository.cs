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
}