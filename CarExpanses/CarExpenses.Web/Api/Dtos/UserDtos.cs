using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Web.Api.Dtos;

public sealed class UserDetailDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IReadOnlyList<CarSummaryDto> Cars { get; set; } = [];
}

public sealed class UserCreateDto
{
    [Required]
    [StringLength(64)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(128)]
    public string Password { get; set; } = string.Empty;
}

public sealed class UserUpdateDto
{
    [Required]
    [StringLength(64)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [StringLength(128)]
    public string? Password { get; set; }
}
