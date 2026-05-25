using CarExpenses.Model;
using Microsoft.AspNetCore.Identity;

namespace CarExpenses.Model.Models;

public class User : IdentityUser<int>, ISoftDeleate
{
    public virtual ICollection<Car>? Cars { get; set; } = new List<Car>();
    public DateTime? DeleatedAt { get; set; }
}
