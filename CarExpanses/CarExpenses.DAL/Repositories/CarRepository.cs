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
}