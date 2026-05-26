using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL.Repositories;

public sealed class UserRepository(CarExpesesDbContext dbContext) : IUserRepository
{
    public IQueryable<User> Query(UserFilter filter)
    {
        var query = dbContext.Users
            .Include(user => user.Cars)
            .AsNoTracking()
            .AsQueryable();

        var term = filter.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(term))
        {
            var hasId = int.TryParse(term, out var idValue);
            query = query.Where(user =>
                (user.UserName != null && user.UserName.Contains(term))
                || (user.Email != null && user.Email.Contains(term))
                || (hasId && user.Id == idValue));
        }

        return query.OrderBy(user => user.Id);
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return dbContext.Users
            .Include(user => user.Cars)
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);
    }
}
