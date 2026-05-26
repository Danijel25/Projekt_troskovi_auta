using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class CarTireRepository(CarExpesesDbContext dbContext) : ICarTireRepository
{
    public IQueryable<CarTire> Query(CarTireFilter filter)
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

        return query.OrderByDescending(item => item.InstalledDate);
    }

    public IReadOnlyList<CarTire> GetAll() => Query(new CarTireFilter()).ToList();

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