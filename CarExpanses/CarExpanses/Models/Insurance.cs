namespace CarExpanses.Models;

public class Insurance
{
    public int Id { get; set; }
    public required string Company { get; set; }
    public required string InsuranceType { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CarId { get; set; }
    public Car? Car { get; set; }

}