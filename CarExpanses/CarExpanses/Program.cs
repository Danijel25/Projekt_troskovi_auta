var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var user1 = new User
{
    Id = 1,
    Username = "marko.kovac",
    Email = "marko.kovac@example.com",
    Password = "Marko2026!",
    Cars = new List<Car>()
};

var car1 = new Car
{
    Id = 101,
    Brand = "Volkswagen",
    Model = "Golf 7",
    Year = 2018,
    EngineVolume = 1.6,
    CurrentMilage = 124500,
    PurchasePrice = 14200m,
    PurchaseDate = new DateTime(2020, 4, 18),
    FuelType = FuelType.Diesel,
    FuelExpenses = new List<FuelExpense>(),
    ServiceRecords = new List<ServiceRecord>(),
    Insurances = new List<Insurance>(),
    CarTires = new List<CarTire>()
};

var tire1 = new Tire
{
    Id = 1001,
    Brand = "Michelin",
    Model = "Primacy 4",
    Season = "Summer",
    Price = 118.50m,
    CarTires = new List<CarTire>()
};

var carTire1 = new CarTire
{
    CarId = car1.Id,
    Car = car1,
    TireId = tire1.Id,
    Tire = tire1,
    InstalledDate = new DateTime(2025, 4, 10)
};

var fuelExpense1 = new FuelExpense
{
    Id = 5001,
    Date = new DateTime(2026, 2, 15),
    Liters = 47.3,
    PricePerLiter = 1.49m,
    TotalCost = 70.48m,
    Milage = 123900,
    CarId = car1.Id,
    Car = car1
};

var serviceRecord1 = new ServiceRecord
{
    Id = 6001,
    ServiceType = "Regular service",
    Description = "Oil and all filters replaced",
    Cost = 185m,
    Date = new DateTime(2025, 11, 20),
    Mileage = 121300,
    CarId = car1.Id,
    Car = car1
};

var insurance1 = new Insurance
{
    Id = 7001,
    Company = "Croatia Osiguranje",
    InsuranceType = "Comprehensive",
    Price = 540m,
    StartDate = new DateTime(2026, 1, 1),
    EndDate = new DateTime(2026, 12, 31),
    CarId = car1.Id,
    Car = car1
};

car1.CarTires.Add(carTire1);
tire1.CarTires.Add(carTire1);
car1.FuelExpenses.Add(fuelExpense1);
car1.ServiceRecords.Add(serviceRecord1);
car1.Insurances.Add(insurance1);
user1.Cars.Add(car1);

var user2 = new User
{
    Id = 2,
    Username = "ana.horvat",
    Email = "ana.horvat@example.com",
    Password = "AnaSecure2026!",
    Cars = new List<Car>()
};

var car2 = new Car
{
    Id = 102,
    Brand = "Toyota",
    Model = "Corolla Hybrid",
    Year = 2021,
    EngineVolume = 1.8,
    CurrentMilage = 68400,
    PurchasePrice = 22600m,
    PurchaseDate = new DateTime(2022, 3, 12),
    FuelType = FuelType.Hybrid,
    FuelExpenses = new List<FuelExpense>(),
    ServiceRecords = new List<ServiceRecord>(),
    Insurances = new List<Insurance>(),
    CarTires = new List<CarTire>()
};

var tire2 = new Tire
{
    Id = 1002,
    Brand = "Goodyear",
    Model = "Vector 4Seasons",
    Season = "All-season",
    Price = 132m,
    CarTires = new List<CarTire>()
};

var carTire2 = new CarTire
{
    CarId = car2.Id,
    Car = car2,
    TireId = tire2.Id,
    Tire = tire2,
    InstalledDate = new DateTime(2025, 9, 5)
};

var fuelExpense2 = new FuelExpense
{
    Id = 5002,
    Date = new DateTime(2026, 2, 21),
    Liters = 35.8,
    PricePerLiter = 1.53m,
    TotalCost = 54.77m,
    Milage = 67810,
    CarId = car2.Id,
    Car = car2
};

var serviceRecord2 = new ServiceRecord
{
    Id = 6002,
    ServiceType = "Brake service",
    Description = "Front brake pads replaced",
    Cost = 240m,
    Date = new DateTime(2025, 10, 2),
    Mileage = 65200,
    CarId = car2.Id,
    Car = car2
};

var insurance2 = new Insurance
{
    Id = 7002,
    Company = "Allianz",
    InsuranceType = "Liability",
    Price = 390m,
    StartDate = new DateTime(2026, 2, 1),
    EndDate = new DateTime(2027, 1, 31),
    CarId = car2.Id,
    Car = car2
};

car2.CarTires.Add(carTire2);
tire2.CarTires.Add(carTire2);
car2.FuelExpenses.Add(fuelExpense2);
car2.ServiceRecords.Add(serviceRecord2);
car2.Insurances.Add(insurance2);
user2.Cars.Add(car2);

var user3 = new User
{
    Id = 3,
    Username = "ivan.babic",
    Email = "ivan.babic@example.com",
    Password = "IvanDrive2026!",
    Cars = new List<Car>()
};

var car3 = new Car
{
    Id = 103,
    Brand = "Tesla",
    Model = "Model 3",
    Year = 2022,
    EngineVolume = 0.0,
    CurrentMilage = 41200,
    PurchasePrice = 38900m,
    PurchaseDate = new DateTime(2023, 7, 1),
    FuelType = FuelType.Electric,
    FuelExpenses = new List<FuelExpense>(),
    ServiceRecords = new List<ServiceRecord>(),
    Insurances = new List<Insurance>(),
    CarTires = new List<CarTire>()
};

var tire3 = new Tire
{
    Id = 1003,
    Brand = "Pirelli",
    Model = "Cinturato P7",
    Season = "Summer",
    Price = 149m,
    CarTires = new List<CarTire>()
};

var carTire3 = new CarTire
{
    CarId = car3.Id,
    Car = car3,
    TireId = tire3.Id,
    Tire = tire3,
    InstalledDate = new DateTime(2025, 3, 25)
};

var fuelExpense3 = new FuelExpense
{
    Id = 5003,
    Date = new DateTime(2026, 3, 1),
    Liters = 0,
    PricePerLiter = 0,
    TotalCost = 18.40m,
    Milage = 40900,
    CarId = car3.Id,
    Car = car3
};

var serviceRecord3 = new ServiceRecord
{
    Id = 6003,
    ServiceType = "Tire rotation",
    Description = "Tires rotated and balanced",
    Cost = 70m,
    Date = new DateTime(2025, 12, 8),
    Mileage = 39500,
    CarId = car3.Id,
    Car = car3
};

var insurance3 = new Insurance
{
    Id = 7003,
    Company = "Wiener",
    InsuranceType = "Comprehensive",
    Price = 810m,
    StartDate = new DateTime(2026, 3, 1),
    EndDate = new DateTime(2027, 2, 28),
    CarId = car3.Id,
    Car = car3
};

car3.CarTires.Add(carTire3);
tire3.CarTires.Add(carTire3);
car3.FuelExpenses.Add(fuelExpense3);
car3.ServiceRecords.Add(serviceRecord3);
car3.Insurances.Add(insurance3);
user3.Cars.Add(car3);

var sampleUsers = new List<User> { user1, user2, user3 };

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

