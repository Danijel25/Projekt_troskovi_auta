using CarExpanses.Models;

namespace CarExpanses.Repositories;

public sealed class CarTireMockRepository
{
    private readonly List<CarTire> _carTires;

    public CarTireMockRepository()
    {
        var carTire1 = new CarTire
        {
            CarId = 101,
            TireId = 1001,
            InstalledDate = new DateTime(2025, 4, 10)
        };

        var carTire2 = new CarTire
        {
            CarId = 102,
            TireId = 1002,
            InstalledDate = new DateTime(2025, 9, 5)
        };

        var carTire3 = new CarTire
        {
            CarId = 103,
            TireId = 1003,
            InstalledDate = new DateTime(2025, 3, 25)
        };

        _carTires = new List<CarTire> { carTire1, carTire2, carTire3 };
    }

    public IReadOnlyList<CarTire> GetAll() => _carTires;

    public CarTire? GetById(int id) => _carTires.FirstOrDefault(carTire => carTire.CarId == id);
}
