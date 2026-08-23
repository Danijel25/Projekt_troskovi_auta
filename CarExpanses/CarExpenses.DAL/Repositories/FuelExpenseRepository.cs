using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class FuelExpenseRepository(CarExpesesDbContext dbContext) : IFuelExpenseRepository
{
    public async Task<IReadOnlyList<FuelExpense>> GetListAsync(FuelExpenseFilter filter)
    {
        var query = dbContext.FuelExpenses
            .Include(fuelExpense => fuelExpense.Car)
            .AsNoTracking()
            .AsQueryable();

        if (filter.CarId.HasValue)
        {
            query = query.Where(item => item.CarId == filter.CarId.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(item => item.FuelExpenseDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(item => item.FuelExpenseDate <= filter.ToDate.Value);
        }

        if (filter.MinLiters.HasValue)
        {
            query = query.Where(item => item.Liters >= filter.MinLiters.Value);
        }

        if (filter.MaxLiters.HasValue)
        {
            query = query.Where(item => item.Liters <= filter.MaxLiters.Value);
        }

        var term = filter.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(term))
        {
            var hasInt = int.TryParse(term, NumberStyles.Integer, CultureInfo.CurrentCulture, out var intValue);
            var hasDecimal = decimal.TryParse(term, NumberStyles.Any, CultureInfo.CurrentCulture, out var decimalValue);
            var hasDate = DateTime.TryParseExact(term, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue)
                || DateTime.TryParse(term, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dateValue);

            query = query.Where(item =>
                (hasInt && (item.Id == intValue || item.CarId == intValue || item.Kilometars == intValue))
                || (hasDecimal && (item.Liters == decimalValue
                    || item.PricePerLiter == decimalValue
                    || (item.Liters * item.PricePerLiter) == decimalValue))
                || (hasDate && item.FuelExpenseDate.Date == dateValue.Date)
                || (item.Car != null && (item.Car.Brand.Contains(term) || item.Car.Model.Contains(term))));
        }

        return await query.OrderByDescending(item => item.FuelExpenseDate).ToListAsync();
    }

    public async Task<IReadOnlyList<FuelExpense>> GetAllAsync() => await GetListAsync(new FuelExpenseFilter());

    public async Task<FuelExpense?> GetByIdAsync(int id) => await dbContext.FuelExpenses
        .Include(fuelExpense => fuelExpense.Car)
        .AsNoTracking()
        .FirstOrDefaultAsync(fuelExpense => fuelExpense.Id == id);

    public async Task<int> AddAsync(FuelExpense fuelExpense)
    {
        dbContext.FuelExpenses.Add(fuelExpense);
        dbContext.SaveChanges();
        return fuelExpense.Id;
    }

    public async Task<bool> UpdateAsync(FuelExpense fuelExpense)
    {
        var existing = dbContext.FuelExpenses.FirstOrDefault(item => item.Id == fuelExpense.Id);
        if (existing is null)
        {
            return false;
        }

        existing.FuelExpenseDate = fuelExpense.FuelExpenseDate;
        existing.Liters = fuelExpense.Liters;
        existing.PricePerLiter = fuelExpense.PricePerLiter;
        existing.Kilometars = fuelExpense.Kilometars;
        existing.CarId = fuelExpense.CarId;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var fuelExpense = await dbContext.FuelExpenses.FirstOrDefaultAsync(item => item.Id == id);
        if (fuelExpense is null)
        {
            return false;
        }

        dbContext.FuelExpenses.Remove(fuelExpense);
        await dbContext.SaveChangesAsync();
        return true;
    }
}