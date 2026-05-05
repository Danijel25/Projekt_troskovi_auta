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
}