using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class ServiceRecordRepository(CarExpesesDbContext dbContext) : IServiceRecordRepository
{
    public async Task<IReadOnlyList<ServiceRecord>> GetLIstAsync(ServiceRecordFilter filter)
    {
        var query = dbContext.ServiceRecords
            .Include(serviceRecord => serviceRecord.Car)
            .AsNoTracking()
            .AsQueryable();

        if (filter.CarId.HasValue)
        {
            query = query.Where(item => item.CarId == filter.CarId.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(item => item.ServiceDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(item => item.ServiceDate <= filter.ToDate.Value);
        }

        var term = filter.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(term))
        {
            var hasInt = int.TryParse(term, NumberStyles.Integer, CultureInfo.CurrentCulture, out var intValue);
            var hasDecimal = decimal.TryParse(term, NumberStyles.Any, CultureInfo.CurrentCulture, out var decimalValue);
            var hasDate = DateTime.TryParseExact(term, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue)
                || DateTime.TryParse(term, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dateValue);

            query = query.Where(item =>
                item.ServiceType.Contains(term)
                || item.Description.Contains(term)
                || (hasDecimal && item.Cost == decimalValue)
                || (hasDate && item.ServiceDate.Date == dateValue.Date)
                || (hasInt && (item.Mileage == intValue || item.CarId == intValue || item.Id == intValue))
                || (item.Car != null && (item.Car.Brand.Contains(term) || item.Car.Model.Contains(term))));
        }

        return await query.OrderByDescending(item => item.ServiceDate).ToListAsync();
    }

    public async Task<IReadOnlyList<ServiceRecord>> GetAllAsync() => await GetLIstAsync(new ServiceRecordFilter());

    public async Task<ServiceRecord?> GetByIdAsync(int id) => await dbContext.ServiceRecords
        .Include(serviceRecord => serviceRecord.Car)
        .AsNoTracking()
        .FirstOrDefaultAsync(serviceRecord => serviceRecord.Id == id);

    public async Task<int> AddAsync(ServiceRecord serviceRecord)
    {
        await dbContext.ServiceRecords.AddAsync(serviceRecord);
        await dbContext.SaveChangesAsync();
        return serviceRecord.Id;
    }

    public async Task<bool> UpdateAsync(ServiceRecord serviceRecord)
    {
        var existing = await dbContext.ServiceRecords.FirstOrDefaultAsync(item => item.Id == serviceRecord.Id);
        if (existing is null)
        {
            return false;
        }

        existing.ServiceType = serviceRecord.ServiceType;
        existing.Description = serviceRecord.Description;
        existing.Cost = serviceRecord.Cost;
        existing.ServiceDate = serviceRecord.ServiceDate;
        existing.Mileage = serviceRecord.Mileage;
        existing.CarId = serviceRecord.CarId;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var serviceRecord = await dbContext.ServiceRecords.FirstOrDefaultAsync(item => item.Id == id);
        if (serviceRecord is null)
        {
            return false;
        }

        dbContext.ServiceRecords.Remove(serviceRecord);
        await dbContext.SaveChangesAsync();
        return true;
    }
}