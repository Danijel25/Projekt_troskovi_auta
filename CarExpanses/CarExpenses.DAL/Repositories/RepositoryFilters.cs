using CarExpenses.Model.Enums;

namespace CarExpenses.DAL.Repositories;

public sealed record CarFilter(
    string? Search = null,
    int? UserId = null,
    FuelType? FuelType = null,
    int? MinYear = null,
    int? MaxYear = null);

public sealed record TireFilter(
    string? Search = null,
    string? Season = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null);

public sealed record CarTireFilter(
    string? Search = null,
    int? CarId = null,
    int? TireId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null);

public sealed record FuelExpenseFilter(
    string? Search = null,
    int? CarId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    decimal? MinLiters = null,
    decimal? MaxLiters = null);

public sealed record ServiceRecordFilter(
    string? Search = null,
    int? CarId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null);

public sealed record InsuranceFilter(
    string? Search = null,
    int? CarId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null);

public sealed record ExpenseCategoryFilter(
    string? Search = null);

public sealed record ExpenseFilter(
    string? Search = null,
    int? CategoryId = null,
    int? CarId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null);

public sealed record UserFilter(
    string? Search = null);
