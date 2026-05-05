using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL.Repositories;

public sealed class CarTireRepository(CarExpesesDbContext dbContext) : ICarTireRepository
{
    public IReadOnlyList<CarTire> GetAll() => dbContext.CarTires
        .Include(carTire => carTire.Car)
        .Include(carTire => carTire.Tire)
        .AsNoTracking()
        .OrderByDescending(carTire => carTire.InstalledDate)
        .ToList();

    public CarTire? GetById(int id) => dbContext.CarTires
        .Include(carTire => carTire.Car)
        .Include(carTire => carTire.Tire)
        .AsNoTracking()
        .FirstOrDefault(carTire => carTire.Id == id);
}