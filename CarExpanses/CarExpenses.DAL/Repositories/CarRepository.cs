using CarExpenses.Model.Enums;
using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class CarRepository(CarExpesesDbContext dbContext) : ICarRepository
{
    public async Task<IReadOnlyList<Car>> GetListAsync(CarFilter filter)
    {
        var query = dbContext.Cars
            .Include(car => car.Expenses)
            .AsNoTracking()
            .AsQueryable();

        if (filter.UserId.HasValue)
        {
            query = query.Where(car => car.UserId == filter.UserId.Value);
        }

        if (filter.FuelType.HasValue)
        {
            query = query.Where(car => car.FuelType == filter.FuelType.Value);
        }

        if (filter.MinYear.HasValue)
        {
            query = query.Where(car => car.Year >= filter.MinYear.Value);
        }

        if (filter.MaxYear.HasValue)
        {
            query = query.Where(car => car.Year <= filter.MaxYear.Value);
        }

        var term = filter.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(term))
        {
            var hasInt = int.TryParse(term, NumberStyles.Integer, CultureInfo.CurrentCulture, out var intValue);
            var hasEngine = double.TryParse(term, NumberStyles.Any, CultureInfo.CurrentCulture, out var engineValue);
            var hasFuelType = Enum.TryParse<FuelType>(term, true, out var fuelTypeValue);

            query = query.Where(car =>
                car.Brand.Contains(term)
                || car.Model.Contains(term)
                || (hasFuelType && car.FuelType == fuelTypeValue)
                || (hasInt && (car.Year == intValue || car.CurrentMilage == intValue || car.Id == intValue))
                || (hasEngine && car.EngineVolume == engineValue));
        }

        return await query.OrderBy(car => car.Id)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Car>> GetAllAsync() => await GetListAsync(new CarFilter());

    public async Task<Car?> GetByIdAsync(int id) => await dbContext.Cars
        .Include(car => car.FuelExpenses)
        .Include(car => car.ServiceRecords)
        .Include(car => car.Insurances)
        .Include(car => car.CarTires!)
            .ThenInclude(carTire => carTire.Tire)
        .Include(car => car.Expenses!)
            .ThenInclude(expense => expense.Category)
        .AsNoTracking()
        .FirstOrDefaultAsync(car => car.Id == id);

    public async Task<int> AddAsync(Car car)
    {
        dbContext.Cars.Add(car);
        await dbContext.SaveChangesAsync();
        return car.Id;
    }

    public async Task<bool> UpdateAsync(Car car)
    {
        var existing = await dbContext.Cars.FirstOrDefaultAsync(item => item.Id == car.Id);
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

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var car = await dbContext.Cars
            .Include(item => item.FuelExpenses)
            .Include(item => item.ServiceRecords)
            .Include(item => item.Insurances)
            .Include(item => item.CarTires!)
                .ThenInclude(carTire => carTire.Tire)
            .Include(item => item.CarFiles)
            .Include(item => item.Expenses!)
                .ThenInclude(expense => expense.Category)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (car is null)
        {
            return false;
        }

        dbContext.Cars.Remove(car);
        await dbContext.SaveChangesAsync();
        return true;
    }
}