using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class InsuranceRepository(CarExpesesDbContext dbContext) : IInsuranceRepository
{
    public async Task<IReadOnlyList<Insurance>> GetListAsync(InsuranceFilter filter)
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

        return await query.OrderByDescending(item => item.StartDate).ToListAsync();
    }

    public Task<IReadOnlyList<Insurance>> GetAllAsync() => GetListAsync(new InsuranceFilter());

    public async Task<Insurance?> GetByIdAsync(int id) => await dbContext.Insurances
        .Include(insurance => insurance.Car)
        .AsNoTracking()
        .FirstOrDefaultAsync(insurance => insurance.Id == id);

    public async Task<int> AddAsync(Insurance insurance)
    {
        await dbContext.Insurances.AddAsync(insurance);
        await dbContext.SaveChangesAsync();
        return insurance.Id;
    }

    public async Task<bool>  UpdateAsync(Insurance insurance)
    {
        var existing = await dbContext.Insurances.FirstOrDefaultAsync(item => item.Id == insurance.Id);
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

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var insurance = await dbContext.Insurances.FirstOrDefaultAsync(item => item.Id == id);
        if (insurance is null)
        {
            return false;
        }

        dbContext.Insurances.Remove(insurance);
        await dbContext.SaveChangesAsync();
        return true;
    }
}