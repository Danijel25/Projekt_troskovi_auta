using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL.Repositories;

public sealed class InsuranceRepository(CarExpesesDbContext dbContext) : IInsuranceRepository
{
    public IReadOnlyList<Insurance> GetAll() => dbContext.Insurances
        .Include(insurance => insurance.Car)
        .AsNoTracking()
        .OrderByDescending(insurance => insurance.StartDate)
        .ToList();

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