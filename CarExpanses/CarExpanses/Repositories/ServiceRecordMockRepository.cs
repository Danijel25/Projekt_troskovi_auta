using CarExpanses.Models;

namespace CarExpanses.Repositories;

public sealed class ServiceRecordMockRepository
{
    private readonly List<ServiceRecord> _serviceRecords =
    [
        new ServiceRecord
        {
            Id = 6001,
            ServiceType = "Regular service",
            Description = "Oil and all filters replaced",
            Cost = 185m,
            ServiceDate = new DateTime(2025, 11, 20),
            Mileage = 121300,
            CarId = 101
        },
        new ServiceRecord
        {
            Id = 6002,
            ServiceType = "Brake service",
            Description = "Front brake pads replaced",
            Cost = 240m,
            ServiceDate = new DateTime(2025, 10, 2),
            Mileage = 65200,
            CarId = 102
        },
        new ServiceRecord
        {
            Id = 6003,
            ServiceType = "Tire rotation",
            Description = "Tires rotated and balanced",
            Cost = 70m,
            ServiceDate = new DateTime(2025, 12, 8),
            Mileage = 39500,
            CarId = 103
        }
    ];

    public IReadOnlyList<ServiceRecord> GetAll() => _serviceRecords;

    public ServiceRecord? GetById(int id) => _serviceRecords.FirstOrDefault(serviceRecord => serviceRecord.Id == id);
}
