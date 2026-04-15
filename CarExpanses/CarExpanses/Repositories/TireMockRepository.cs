using CarExpanses.Models;

namespace CarExpanses.Repositories;

public sealed class TireMockRepository
{
    private readonly List<Tire> _tires =
    [
        new Tire
        {
            Id = 1001,
            Brand = "Michelin",
            Model = "Primacy 4",
            Season = "Summer",
            Price = 118.50m,
            CarTires = new List<CarTire>()
        },
        new Tire
        {
            Id = 1002,
            Brand = "Goodyear",
            Model = "Vector 4Seasons",
            Season = "All-season",
            Price = 132m,
            CarTires = new List<CarTire>()
        },
        new Tire
        {
            Id = 1003,
            Brand = "Pirelli",
            Model = "Cinturato P7",
            Season = "Summer",
            Price = 149m,
            CarTires = new List<CarTire>()
        }
    ];

    public IReadOnlyList<Tire> GetAll() => _tires;

    public Tire? GetById(int id) => _tires.FirstOrDefault(tire => tire.Id == id);
}
