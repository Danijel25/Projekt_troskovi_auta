using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class TireRepository(CarExpesesDbContext dbContext) : ITireRepository
{
    public IQueryable<Tire> Query(TireFilter filter)
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

        return query.OrderBy(tire => tire.Id);
    }

    public IReadOnlyList<Tire> GetAll() => Query(new TireFilter()).ToList();

    public Tire? GetById(int id) => dbContext.Tires
        .Include(tire => tire.CarTires)
        .AsNoTracking()
        .FirstOrDefault(tire => tire.Id == id);

    public void Add(Tire tire)
    {
        dbContext.Tires.Add(tire);
        dbContext.SaveChanges();
    }

    public bool Update(Tire tire)
    {
        var existing = dbContext.Tires.FirstOrDefault(item => item.Id == tire.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Brand = tire.Brand;
        existing.Model = tire.Model;
        existing.Season = tire.Season;
        existing.Price = tire.Price;

        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var tire = dbContext.Tires
            .Include(item => item.CarTires)
            .FirstOrDefault(item => item.Id == id);

        if (tire is null)
        {
            return false;
        }

        dbContext.Tires.Remove(tire);
        dbContext.SaveChanges();
        return true;
    }
}