using CarExpanses.Models;
using CarExpanses.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<UserMockRepository>();
builder.Services.AddSingleton<CarMockRepository>();
builder.Services.AddSingleton<TireMockRepository>();
builder.Services.AddSingleton<CarTireMockRepository>();
builder.Services.AddSingleton<FuelExpenseMockRepository>();
builder.Services.AddSingleton<ServiceRecordMockRepository>();
builder.Services.AddSingleton<InsuranceMockRepository>();
builder.Services.AddSingleton<ExpenseCategoryMockRepository>();
builder.Services.AddSingleton<ExpenseMockRepository>();

var app = builder.Build();
var sampleUsers = app.Services.GetRequiredService<UserMockRepository>().GetAll();

var carTotalCosts = sampleUsers
    .SelectMany(user => user.Cars ?? new List<Car>())
    .Select(car => new
    {
        CarName = $"{car.Brand} {car.Model}",
        TotalCost =
            (car.FuelExpenses?.Sum(fuel => fuel.TotalCost) ?? 0m) +
            (car.ServiceRecords?.Sum(service => service.Cost) ?? 0m) +
            (car.Insurances?.Sum(insurance => insurance.Price) ?? 0m) +
            (car.Expenses?.Sum(expense => expense.Amount) ?? 0m) +
            (car.CarTires?.Sum(carTire => carTire.Tire?.Price ?? 0m) ?? 0m)
    })
    .OrderByDescending(carCost => carCost.TotalCost)
    .ToList();

Console.WriteLine("Total costs per car:");
foreach (var carCost in carTotalCosts)
{
    Console.WriteLine($"{carCost.CarName}: {carCost.TotalCost:C}");
}

var thirtyDaysAgo = DateTime.Now.AddDays(-30);

var recentFuelExpenses = sampleUsers
    .SelectMany(user => user.Cars ?? new List<Car>())
    .SelectMany(car => car.FuelExpenses ?? new List<FuelExpense>())
    .Where(fuelExpense => fuelExpense.FuelExpenseDate >= thirtyDaysAgo && fuelExpense.FuelExpenseDate <= DateTime.Now)
    .ToList();

Console.WriteLine("\nFuel expenses in the last 30 days:");
foreach (var fuelExpense in recentFuelExpenses)
{
    var brand = fuelExpense.Car?.Brand ?? "Unknown";
    var model = fuelExpense.Car?.Model ?? "Car";
    Console.WriteLine($"{brand} {model} - {fuelExpense.FuelExpenseDate.ToShortDateString()}: {fuelExpense.TotalCost:C}");
}

var targetBrand = "Toyota";

var firstServiceRecordForBrand = sampleUsers
    .SelectMany(user => user.Cars ?? new List<Car>())
    .Where(car => car.Brand.Equals(targetBrand, StringComparison.OrdinalIgnoreCase))
    .SelectMany(car => car.ServiceRecords ?? new List<ServiceRecord>())
    .OrderBy(serviceRecord => serviceRecord.ServiceDate)
    .FirstOrDefault();

Console.WriteLine($"\nFirst service record for brand '{targetBrand}':");
if (firstServiceRecordForBrand is null)
{
    Console.WriteLine("No service records found.");
}
else
{
    var brand = firstServiceRecordForBrand.Car?.Brand ?? "Unknown";
    var model = firstServiceRecordForBrand.Car?.Model ?? "Car";
    Console.WriteLine($"{brand} {model} - {firstServiceRecordForBrand.ServiceType} on {firstServiceRecordForBrand.ServiceDate.ToShortDateString()} costing {firstServiceRecordForBrand.Cost:C}");
}


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

