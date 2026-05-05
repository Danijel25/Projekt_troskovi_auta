using CarExpenses.Model.Enums;
using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL;

public class CarExpesesDbContext : DbContext
{
    public CarExpesesDbContext(DbContextOptions<CarExpesesDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Tire> Tires { get; set; }
    public DbSet<CarTire> CarTires { get; set; }
    public DbSet<FuelExpense> FuelExpenses { get; set; }
    public DbSet<ServiceRecord> ServiceRecords { get; set; }
    public DbSet<Insurance> Insurances { get; set; }
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "marko92",
                Email = "marko.pavic@example.com",
                Password = "Pass123!"
            },
            new User
            {
                Id = 2,
                Username = "ivana87",
                Email = "ivana.horvat@example.com",
                Password = "Pass123!"
            },
            new User
            {
                Id = 3,
                Username = "petra95",
                Email = "petra.kovac@example.com",
                Password = "Pass123!"
            });

        modelBuilder.Entity<Car>().HasData(
            new Car
            {
                Id = 1,
                UserId = 1,
                Brand = "Toyota",
                Model = "Corolla",
                Year = 2021,
                EngineVolume = 1.8,
                CurrentMilage = 68500,
                PurchasePrice = 19850m,
                PurchaseDate = new DateTime(2021, 3, 12),
                FuelType = FuelType.Hybrid
            },
            new Car
            {
                Id = 2,
                UserId = 2,
                Brand = "BMW",
                Model = "320d",
                Year = 2019,
                EngineVolume = 2.0,
                CurrentMilage = 112300,
                PurchasePrice = 27900m,
                PurchaseDate = new DateTime(2019, 7, 8),
                FuelType = FuelType.Diesel
            },
            new Car
            {
                Id = 3,
                UserId = 3,
                Brand = "Tesla",
                Model = "Model 3",
                Year = 2022,
                EngineVolume = 0,
                CurrentMilage = 43100,
                PurchasePrice = 43990m,
                PurchaseDate = new DateTime(2022, 11, 22),
                FuelType = FuelType.Electric
            });

        modelBuilder.Entity<Tire>().HasData(
            new Tire
            {
                Id = 1,
                Brand = "Michelin",
                Model = "Pilot Sport 5",
                Season = "Summer",
                Price = 145m
            },
            new Tire
            {
                Id = 2,
                Brand = "Continental",
                Model = "PremiumContact 6",
                Season = "Summer",
                Price = 138m
            },
            new Tire
            {
                Id = 3,
                Brand = "Bridgestone",
                Model = "Blizzak LM005",
                Season = "Winter",
                Price = 152m
            });

        modelBuilder.Entity<CarTire>().HasData(
            new CarTire
            {
                Id = 1,
                CarId = 1,
                TireId = 1,
                InstalledDate = new DateTime(2025, 3, 10)
            },
            new CarTire
            {
                Id = 2,
                CarId = 2,
                TireId = 2,
                InstalledDate = new DateTime(2025, 4, 3)
            },
            new CarTire
            {
                Id = 3,
                CarId = 3,
                TireId = 3,
                InstalledDate = new DateTime(2025, 11, 5)
            });

        modelBuilder.Entity<ExpenseCategory>().HasData(
            new ExpenseCategory
            {
                Id = 1,
                Name = "Car washing"
            },
            new ExpenseCategory
            {
                Id = 2,
                Name = "Wiper fluid refill"
            },
            new ExpenseCategory
            {
                Id = 3,
                Name = "Parking and tolls"
            });

        modelBuilder.Entity<FuelExpense>().HasData(
            new FuelExpense
            {
                Id = 1,
                FuelExpenseDate = new DateTime(2026, 1, 10),
                Liters = 42.50m,
                PricePerLiter = 1.63m,
                Kilometars = 68940,
                CarId = 1
            },
            new FuelExpense
            {
                Id = 2,
                FuelExpenseDate = new DateTime(2026, 1, 15),
                Liters = 55.10m,
                PricePerLiter = 1.58m,
                Kilometars = 113020,
                CarId = 2
            },
            new FuelExpense
            {
                Id = 3,
                FuelExpenseDate = new DateTime(2026, 1, 20),
                Liters = 0m,
                PricePerLiter = 0m,
                Kilometars = 43580,
                CarId = 3
            });

        modelBuilder.Entity<ServiceRecord>().HasData(
            new ServiceRecord
            {
                Id = 1,
                ServiceType = "Regular maintenance",
                Description = "Oil and filter change",
                Cost = 180m,
                ServiceDate = new DateTime(2026, 2, 5),
                Mileage = 69210,
                CarId = 1
            },
            new ServiceRecord
            {
                Id = 2,
                ServiceType = "Brake service",
                Description = "Front brake pads replacement",
                Cost = 320m,
                ServiceDate = new DateTime(2026, 2, 7),
                Mileage = 113450,
                CarId = 2
            },
            new ServiceRecord
            {
                Id = 3,
                ServiceType = "Battery check",
                Description = "High-voltage system diagnostic",
                Cost = 95m,
                ServiceDate = new DateTime(2026, 2, 9),
                Mileage = 43820,
                CarId = 3
            });

        modelBuilder.Entity<Insurance>().HasData(
            new Insurance
            {
                Id = 1,
                Company = "Allianz",
                InsuranceType = "Comprehensive",
                Price = 640m,
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 12, 31),
                CarId = 1
            },
            new Insurance
            {
                Id = 2,
                Company = "Generali",
                InsuranceType = "Comprehensive",
                Price = 760m,
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 12, 31),
                CarId = 2
            },
            new Insurance
            {
                Id = 3,
                Company = "Wiener Osiguranje",
                InsuranceType = "Comprehensive",
                Price = 890m,
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 12, 31),
                CarId = 3
            });

        modelBuilder.Entity<Expense>().HasData(
            new
            {
                Id = 1,
                Description = "Hand wash and interior cleaning",
                Amount = 18m,
                Date = new DateTime(2026, 1, 12),
                CategoryId = 1,
                CarId = 1
            },
            new
            {
                Id = 2,
                Description = "Winter windshield washer fluid",
                Amount = 7.5m,
                Date = new DateTime(2026, 1, 18),
                CategoryId = 2,
                CarId = 2
            },
            new
            {
                Id = 3,
                Description = "Monthly garage parking pass",
                Amount = 62m,
                Date = new DateTime(2026, 1, 25),
                CategoryId = 3,
                CarId = 3
            });
    }
}