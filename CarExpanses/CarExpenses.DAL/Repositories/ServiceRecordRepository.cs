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
}