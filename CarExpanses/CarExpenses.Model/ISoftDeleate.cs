using System;

namespace CarExpenses.Model
{
    public interface ISoftDeleate
    {
        DateTime? DeleatedAt { get; set; }
    }
}
