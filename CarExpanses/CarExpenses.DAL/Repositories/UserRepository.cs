using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL.Repositories;

public sealed class UserRepository(CarExpesesDbContext dbContext) : IUserRepository
{
    public IReadOnlyList<User> GetAll() => dbContext.Users
        .Include(user => user.Cars)
        .AsNoTracking()
        .OrderBy(user => user.Id)
        .ToList();

    public User? GetById(int id) => dbContext.Users
        .Include(user => user.Cars)
        .AsNoTracking()
        .FirstOrDefault(user => user.Id == id);

    public User? GetByIdWithDetails(int id) => dbContext.Users
        .Include(user => user.Cars)!
            .ThenInclude(car => car.FuelExpenses)
        .Include(user => user.Cars)!
            .ThenInclude(car => car.ServiceRecords)
        .Include(user => user.Cars)!
            .ThenInclude(car => car.Insurances)
        .Include(user => user.Cars)!
            .ThenInclude(car => car.CarTires)!
                .ThenInclude(carTire => carTire.Tire)
        .Include(user => user.Cars)!
            .ThenInclude(car => car.Expenses)
        .AsNoTracking()
        .FirstOrDefault(user => user.Id == id);

    public void Add(User user)
    {
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
    }

    public bool Update(User user)
    {
        var existing = dbContext.Users.FirstOrDefault(item => item.Id == user.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Username = user.Username;
        existing.Email = user.Email;
        existing.Password = user.Password;

        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var user = dbContext.Users
            .Include(item => item.Cars)!
                .ThenInclude(car => car.FuelExpenses)
            .Include(item => item.Cars)!
                .ThenInclude(car => car.ServiceRecords)
            .Include(item => item.Cars)!
                .ThenInclude(car => car.Insurances)
            .Include(item => item.Cars)!
                .ThenInclude(car => car.CarTires)!
                    .ThenInclude(carTire => carTire.Tire)
            .Include(item => item.Cars)!
                .ThenInclude(car => car.CarFiles)
            .Include(item => item.Cars)!
                .ThenInclude(car => car.Expenses)
            .FirstOrDefault(item => item.Id == id);

        if (user is null)
        {
            return false;
        }

        dbContext.Users.Remove(user);
        dbContext.SaveChanges();
        return true;
    }
}