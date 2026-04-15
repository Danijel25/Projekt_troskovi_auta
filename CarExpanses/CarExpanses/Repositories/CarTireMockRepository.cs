using CarExpanses.Models;

namespace CarExpanses.Repositories;

public sealed class CarTireMockRepository
{
    private readonly List<CarTire> _carTires =
    [
        new CarTire
        {
            CarId = 101,
            TireId = 1001,
            InstalledDate = new DateTime(2025, 4, 10)
        },
        new CarTire
        {
            CarId = 102,
            TireId = 1002,
            InstalledDate = new DateTime(2025, 9, 5)
        },
        new CarTire
        {
            CarId = 103,
            TireId = 1003,
            InstalledDate = new DateTime(2025, 3, 25)
        }
    ];

    public IReadOnlyList<CarTire> GetAll() => _carTires;

    public CarTire? GetById(int id) => _carTires.FirstOrDefault(carTire => carTire.CarId == id);
}
