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

    public void Add(CarTire carTire)
    {
        dbContext.CarTires.Add(carTire);
        dbContext.SaveChanges();
    }

    public bool Update(CarTire carTire)
    {
        var existing = dbContext.CarTires.FirstOrDefault(item => item.Id == carTire.Id);
        if (existing is null)
        {
            return false;
        }

        existing.CarId = carTire.CarId;
        existing.TireId = carTire.TireId;
        existing.InstalledDate = carTire.InstalledDate;

        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var carTire = dbContext.CarTires.FirstOrDefault(item => item.Id == id);
        if (carTire is null)
        {
            return false;
        }

        dbContext.CarTires.Remove(carTire);
        dbContext.SaveChanges();
        return true;
    }
}