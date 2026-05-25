using CarExpenses.Model;
using CarExpenses.Model.Enums;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CarExpenses.DAL;

public class CarExpesesDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    private readonly ICurrentUserService currentUserService;

    public CarExpesesDbContext(DbContextOptions<CarExpesesDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        this.currentUserService = currentUserService;
    }

    public DbSet<Car> Cars { get; set; }
    public DbSet<CarFile> CarFiles { get; set; }
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

        var adminRoleId = 1;
        var basicRoleId = 2;
        var adminRole = new IdentityRole<int>
        {
            Id = adminRoleId,
            Name = AppRoles.Admin,
            NormalizedName = AppRoles.Admin.ToUpperInvariant(),
            ConcurrencyStamp = "990b74f8-2b93-47c4-b9eb-f20143ce7233"
        };
        var basicRole = new IdentityRole<int>
        {
            Id = basicRoleId,
            Name = AppRoles.BasicUser,
            NormalizedName = AppRoles.BasicUser.ToUpperInvariant(),
            ConcurrencyStamp = "5c0b4d43-5b34-4766-8cbd-eec28d7d7e46"
        };

        // var hasher = new PasswordHasher<User>();
        var user1 = new User
        {
            Id = 1,
            UserName = "marko92",
            NormalizedUserName = "MARKO92",
            Email = "marko.pavic@example.com",
            NormalizedEmail = "MARKO.PAVIC@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = "5bc64388-da3b-402a-9983-cce9653f7d07",
            ConcurrencyStamp = "4547430a-6aff-4f30-9957-467c65996ed8"
        };
        user1.PasswordHash = "123";

        var user2 = new User
        {
            Id = 2,
            UserName = "ivana87",
            NormalizedUserName = "IVANA87",
            Email = "ivana.horvat@example.com",
            NormalizedEmail = "IVANA.HORVAT@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = "fa552fcd-8a5e-4c44-95bc-091a2e7e0c18",
            ConcurrencyStamp = "f37c9416-0ab1-4efb-b72a-867589f227be"
        };
        user2.PasswordHash = "Pass123!";

        var user3 = new User
        {
            Id = 3,
            UserName = "petra95",
            NormalizedUserName = "PETRA95",
            Email = "petra.kovac@example.com",
            NormalizedEmail = "PETRA.KOVAC@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = "cc932eb4-92c4-4595-8994-76ae2a599f40",
            ConcurrencyStamp = "f09f1059-3eed-445c-a079-5a1e31ee9fa3"
        };
        user3.PasswordHash = "Pass123!";

        modelBuilder.Entity<IdentityRole<int>>().HasData(adminRole, basicRole);
        modelBuilder.Entity<User>().HasData(user1, user2, user3);
        modelBuilder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int> { UserId = user1.Id, RoleId = adminRoleId },
            new IdentityUserRole<int> { UserId = user2.Id, RoleId = basicRoleId },
            new IdentityUserRole<int> { UserId = user3.Id, RoleId = basicRoleId });

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

        modelBuilder.Entity<User>().HasQueryFilter(user => !user.DeleatedAt.HasValue);
        modelBuilder.Entity<Car>().HasQueryFilter(car => !car.DeleatedAt.HasValue && (IsAdmin || (CurrentUserId.HasValue && car.UserId == CurrentUserId)));
        modelBuilder.Entity<CarFile>().HasQueryFilter(file => !file.DeleatedAt.HasValue && (IsAdmin || (CurrentUserId.HasValue && file.Car != null && file.Car.UserId == CurrentUserId)));
        modelBuilder.Entity<FuelExpense>().HasQueryFilter(expense => !expense.DeleatedAt.HasValue && (IsAdmin || (CurrentUserId.HasValue && expense.Car != null && expense.Car.UserId == CurrentUserId)));
        modelBuilder.Entity<ServiceRecord>().HasQueryFilter(record => !record.DeleatedAt.HasValue && (IsAdmin || (CurrentUserId.HasValue && record.Car != null && record.Car.UserId == CurrentUserId)));
        modelBuilder.Entity<Insurance>().HasQueryFilter(insurance => !insurance.DeleatedAt.HasValue && (IsAdmin || (CurrentUserId.HasValue && insurance.Car != null && insurance.Car.UserId == CurrentUserId)));
        modelBuilder.Entity<CarTire>().HasQueryFilter(carTire => !carTire.DeleatedAt.HasValue && (IsAdmin || (CurrentUserId.HasValue && carTire.Car != null && carTire.Car.UserId == CurrentUserId)));
        modelBuilder.Entity<Expense>().HasQueryFilter(expense => !expense.DeleatedAt.HasValue && (IsAdmin || (CurrentUserId.HasValue && expense.Car != null && expense.Car.UserId == CurrentUserId)));
        modelBuilder.Entity<Tire>().HasQueryFilter(tire => !tire.DeleatedAt.HasValue);
        modelBuilder.Entity<ExpenseCategory>().HasQueryFilter(category => !category.DeleatedAt.HasValue);
    }

    private int? CurrentUserId => currentUserService.UserId;

    private bool IsAdmin => currentUserService.IsInRole(AppRoles.Admin);


    private void UpdateSoftDeleteStatuses()
    {
        var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted && e.Entity is ISoftDeleate).ToList();
        foreach (var entry in entries)
        {
            var entity = (ISoftDeleate)entry.Entity;
            entity.DeleatedAt = DateTime.UtcNow;
            entry.State = EntityState.Modified;
        }
    }

    public override int SaveChanges()
    {
        UpdateSoftDeleteStatuses();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateSoftDeleteStatuses();
        return base.SaveChangesAsync(cancellationToken);
    }
}