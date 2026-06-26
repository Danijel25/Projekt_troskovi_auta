using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class CarTireRepository(CarExpesesDbContext dbContext) : ICarTireRepository
{
    public async Task<IReadOnlyList<CarTire>> GetListAsync(CarTireFilter filter)
    {
        var query = dbContext.CarTires
            .Include(carTire => carTire.Car)
            .Include(carTire => carTire.Tire)
            .AsNoTracking()
            .AsQueryable();

        if (filter.CarId.HasValue)
        {
            query = query.Where(item => item.CarId == filter.CarId.Value);
        }

        if (filter.TireId.HasValue)
        {
            query = query.Where(item => item.TireId == filter.TireId.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(item => item.InstalledDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(item => item.InstalledDate <= filter.ToDate.Value);
        }

        var term = filter.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(term))
        {
            var hasInt = int.TryParse(term, NumberStyles.Integer, CultureInfo.CurrentCulture, out var intValue);
            var hasDate = DateTime.TryParseExact(term, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue)
                || DateTime.TryParse(term, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dateValue);

            query = query.Where(item =>
                (hasInt && (item.Id == intValue || item.CarId == intValue || item.TireId == intValue))
                || (hasDate && item.InstalledDate.Date == dateValue.Date)
                || (item.Car != null && (item.Car.Brand.Contains(term) || item.Car.Model.Contains(term)))
                || (item.Tire != null && (item.Tire.Brand.Contains(term)
                    || item.Tire.Model.Contains(term)
                    || item.Tire.Season.Contains(term))));
        }

        return await query
            .OrderByDescending(item => item.InstalledDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<CarTire>> GetAllAsync() => await GetListAsync(new CarTireFilter());

    public async Task<CarTire?> GetByIdAsync(int id) => await dbContext.CarTires
        .Include(carTire => carTire.Car)
        .Include(carTire => carTire.Tire)
        .AsNoTracking()
        .FirstOrDefaultAsync(carTire => carTire.Id == id);

    public async Task<int> AddAsync(CarTire carTire)
    {
        dbContext.CarTires.Add(carTire);
        await dbContext.SaveChangesAsync();
        return carTire.Id;
    }

    public async Task<bool> UpdateAsync(CarTire carTire)
    {
        var existing = await dbContext.CarTires.FirstOrDefaultAsync(item => item.Id == carTire.Id);
        if (existing is null)
        {
            return false;
        }

        existing.CarId = carTire.CarId;
        existing.TireId = carTire.TireId;
        existing.InstalledDate = carTire.InstalledDate;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var carTire = await dbContext.CarTires.FirstOrDefaultAsync(item => item.Id == id);
        if (carTire is null)
        {
            return false;
        }

        dbContext.CarTires.Remove(carTire);
        await dbContext.SaveChangesAsync();
        return true;
    }
}