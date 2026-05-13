using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL.Repositories;

public sealed class ServiceRecordRepository(CarExpesesDbContext dbContext) : IServiceRecordRepository
{
    public IReadOnlyList<ServiceRecord> GetAll() => dbContext.ServiceRecords
        .Include(serviceRecord => serviceRecord.Car)
        .AsNoTracking()
        .OrderByDescending(serviceRecord => serviceRecord.ServiceDate)
        .ToList();

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