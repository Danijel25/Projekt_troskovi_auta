using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class ServiceRecordRepository(CarExpesesDbContext dbContext) : IServiceRecordRepository
{
    public IQueryable<ServiceRecord> Query(ServiceRecordFilter filter)
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

        return query.OrderByDescending(item => item.ServiceDate);
    }

    public IReadOnlyList<ServiceRecord> GetAll() => Query(new ServiceRecordFilter()).ToList();

    public ServiceRecord? GetById(int id) => dbContext.ServiceRecords
        .Include(serviceRecord => serviceRecord.Car)
        .AsNoTracking()
        .FirstOrDefault(serviceRecord => serviceRecord.Id == id);

    public void Add(ServiceRecord serviceRecord)
    {
        dbContext.ServiceRecords.Add(serviceRecord);
        dbContext.SaveChanges();
    }

    public bool Update(ServiceRecord serviceRecord)
    {
        var existing = dbContext.ServiceRecords.FirstOrDefault(item => item.Id == serviceRecord.Id);
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

        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var serviceRecord = dbContext.ServiceRecords.FirstOrDefault(item => item.Id == id);
        if (serviceRecord is null)
        {
            return false;
        }

        dbContext.ServiceRecords.Remove(serviceRecord);
        dbContext.SaveChanges();
        return true;
    }
}