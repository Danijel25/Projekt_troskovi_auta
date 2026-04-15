using CarExpanses.Models;

namespace CarExpanses.Repositories;

public sealed class InsuranceMockRepository
{
    private readonly List<Insurance> _insurances =
    [
        new Insurance
        {
            Id = 7001,
            Company = "Croatia Osiguranje",
            InsuranceType = "Comprehensive",
            Price = 540m,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            CarId = 101
        },
        new Insurance
        {
            Id = 7002,
            Company = "Allianz",
            InsuranceType = "Liability",
            Price = 390m,
            StartDate = new DateTime(2026, 2, 1),
            EndDate = new DateTime(2027, 1, 31),
            CarId = 102
        },
        new Insurance
        {
            Id = 7003,
            Company = "Wiener",
            InsuranceType = "Comprehensive",
            Price = 810m,
            StartDate = new DateTime(2026, 3, 1),
            EndDate = new DateTime(2027, 2, 28),
            CarId = 103
        }
    ];

    public IReadOnlyList<Insurance> GetAll() => _insurances;

    public Insurance? GetById(int id) => _insurances.FirstOrDefault(insurance => insurance.Id == id);
}
