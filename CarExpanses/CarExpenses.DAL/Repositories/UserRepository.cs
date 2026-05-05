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
}