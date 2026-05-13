using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL.Repositories;

public sealed class CarRepository(CarExpesesDbContext dbContext) : ICarRepository
{
    public IReadOnlyList<Car> GetAll() => dbContext.Cars
        .Include(car => car.Expenses)
        .AsNoTracking()
        .OrderBy(car => car.Id)
        .ToList();

    public Car? GetById(int id) => dbContext.Cars
        .Include(car => car.FuelExpenses)
        .Include(car => car.ServiceRecords)
        .Include(car => car.Insurances)
        .Include(car => car.CarTires!)
            .ThenInclude(carTire => carTire.Tire)
        .Include(car => car.Expenses!)
            .ThenInclude(expense => expense.Category)
        .AsNoTracking()
        .FirstOrDefault(car => car.Id == id);

    public void Add(Car car)
    {
        dbContext.Cars.Add(car);
        dbContext.SaveChanges();
    }

    public bool Update(Car car)
    {
        var existing = dbContext.Cars.FirstOrDefault(item => item.Id == car.Id);
        if (existing is null)
        {
            return false;
        }

        existing.UserId = car.UserId;
        existing.Brand = car.Brand;
        existing.Model = car.Model;
        existing.Year = car.Year;
        existing.EngineVolume = car.EngineVolume;
        existing.CurrentMilage = car.CurrentMilage;
        existing.PurchasePrice = car.PurchasePrice;
        existing.PurchaseDate = car.PurchaseDate;
        existing.FuelType = car.FuelType;

        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var car = dbContext.Cars
            .Include(item => item.FuelExpenses)
            .Include(item => item.ServiceRecords)
            .Include(item => item.Insurances)
            .Include(item => item.CarTires!)
                .ThenInclude(carTire => carTire.Tire)
            .Include(item => item.Expenses!)
                .ThenInclude(expense => expense.Category)
            .FirstOrDefault(item => item.Id == id);

        if (car is null)
        {
            return false;
        }

        dbContext.Cars.Remove(car);
        dbContext.SaveChanges();
        return true;
    }
}