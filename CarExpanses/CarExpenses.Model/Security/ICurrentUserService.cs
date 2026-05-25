namespace CarExpenses.Model.Security;

public interface ICurrentUserService
{
    int? UserId { get; }
    bool IsInRole(string role);
}
