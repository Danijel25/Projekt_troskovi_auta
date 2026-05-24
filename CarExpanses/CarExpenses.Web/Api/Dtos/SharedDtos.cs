using CarExpenses.Model.Enums;

namespace CarExpenses.Web.Api.Dtos;

public sealed class UserSummaryDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public sealed class CarSummaryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public FuelType FuelType { get; set; }
    public int CurrentMilage { get; set; }
}

public sealed class TireSummaryDto
{
    public int Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public sealed class ExpenseCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class ExpenseSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
