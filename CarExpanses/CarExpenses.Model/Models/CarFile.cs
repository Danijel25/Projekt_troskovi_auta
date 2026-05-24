using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarExpenses.Model;

namespace CarExpenses.Model.Models;

public class CarFile : ISoftDeleate
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Car")]
    public int CarId { get; set; }

    public virtual Car? Car { get; set; }

    [Required]
    [StringLength(256)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(128)]
    public string StoredFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(128)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    [Required]
    [StringLength(256)]
    public string RelativePath { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }

    public DateTime? DeleatedAt { get; set; }
}
