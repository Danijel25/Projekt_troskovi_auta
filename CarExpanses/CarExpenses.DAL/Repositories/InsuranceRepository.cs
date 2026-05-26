using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class InsuranceRepository(CarExpesesDbContext dbContext) : IInsuranceRepository
{
    public IQueryable<Insurance> Query(InsuranceFilter filter)
    {
        var query = dbContext.Insurances
            .Include(insurance => insurance.Car)
            .AsNoTracking()
            .AsQueryable();

        if (filter.CarId.HasValue)
        {
            query = query.Where(item => item.CarId == filter.CarId.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(item => item.StartDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(item => item.EndDate <= filter.ToDate.Value);
        }

        var term = filter.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(term))
        {
            var hasInt = int.TryParse(term, NumberStyles.Integer, CultureInfo.CurrentCulture, out var intValue);
            var hasDecimal = decimal.TryParse(term, NumberStyles.Any, CultureInfo.CurrentCulture, out var decimalValue);
            var hasDate = DateTime.TryParseExact(term, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue)
                || DateTime.TryParse(term, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dateValue);

            query = query.Where(item =>
                item.Company.Contains(term)
                || item.InsuranceType.Contains(term)
                || (hasDecimal && item.Price == decimalValue)
                || (hasDate && (item.StartDate.Date == dateValue.Date || item.EndDate.Date == dateValue.Date))
                || (hasInt && (item.CarId == intValue || item.Id == intValue))
                || (item.Car != null && (item.Car.Brand.Contains(term) || item.Car.Model.Contains(term))));
        }

        return query.OrderByDescending(item => item.StartDate);
    }

    public IReadOnlyList<Insurance> GetAll() => Query(new InsuranceFilter()).ToList();

    public Insurance? GetById(int id) => dbContext.Insurances
        .Include(insurance => insurance.Car)
        .AsNoTracking()
        .FirstOrDefault(insurance => insurance.Id == id);

    public void Add(Insurance insurance)
    {
        dbContext.Insurances.Add(insurance);
        dbContext.SaveChanges();
    }

    public bool Update(Insurance insurance)
    {
        var existing = dbContext.Insurances.FirstOrDefault(item => item.Id == insurance.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Company = insurance.Company;
        existing.InsuranceType = insurance.InsuranceType;
        existing.Price = insurance.Price;
        existing.StartDate = insurance.StartDate;
        existing.EndDate = insurance.EndDate;
        existing.CarId = insurance.CarId;

        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var insurance = dbContext.Insurances.FirstOrDefault(item => item.Id == id);
        if (insurance is null)
        {
            return false;
        }

        dbContext.Insurances.Remove(insurance);
        dbContext.SaveChanges();
        return true;
    }
}