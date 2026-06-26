using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class TireRepository(CarExpesesDbContext dbContext) : ITireRepository
{
    public async Task<IReadOnlyList<Tire>> GetListAsync(TireFilter filter)
    {
        var query = dbContext.Tires
            .Include(tire => tire.CarTires)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Season))
        {
            var season = filter.Season.Trim();
            query = query.Where(tire => tire.Season.Contains(season));
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(tire => tire.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(tire => tire.Price <= filter.MaxPrice.Value);
        }

        var term = filter.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(term))
        {
            var hasId = int.TryParse(term, NumberStyles.Integer, CultureInfo.CurrentCulture, out var idValue);
            var hasPrice = decimal.TryParse(term, NumberStyles.Any, CultureInfo.CurrentCulture, out var priceValue);

            query = query.Where(tire =>
                tire.Brand.Contains(term)
                || tire.Model.Contains(term)
                || tire.Season.Contains(term)
                || (hasId && tire.Id == idValue)
                || (hasPrice && tire.Price == priceValue));
        }

        return await query.OrderBy(tire => tire.Id).ToListAsync();
    }

    public async Task<IReadOnlyList<Tire>> GetAllAsync() => await GetListAsync(new TireFilter());

    public async Task<Tire?> GetByIdAsync(int id) => await dbContext.Tires
        .Include(tire => tire.CarTires)
        .AsNoTracking()
        .FirstOrDefaultAsync(tire => tire.Id == id);

    public async Task<int> AddAsync(Tire tire)
    {
        dbContext.Tires.Add(tire);
        await dbContext.SaveChangesAsync();
        return tire.Id;
    }

    public async Task<bool> UpdateAsync(Tire tire)
    {
        var existing = await dbContext.Tires.FirstOrDefaultAsync(item => item.Id == tire.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Brand = tire.Brand;
        existing.Model = tire.Model;
        existing.Season = tire.Season;
        existing.Price = tire.Price;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool>  DeleteAsync(int id)
    {
        var tire = await dbContext.Tires
            .Include(item => item.CarTires)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (tire is null)
        {
            return false;
        }

        dbContext.Tires.Remove(tire);
        await dbContext.SaveChangesAsync();
        return true;
    }
}