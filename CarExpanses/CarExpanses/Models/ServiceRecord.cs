namespace CarExpanses.Models;

public class ServiceRecord
{
    public int Id { get; set; }
    public required string ServiceType { get; set; }
    public required string Description { get; set; }
    public decimal Cost { get; set; }
    public DateTime ServiceDate { get; set; }
    public int Mileage { get; set; }
    public int CarId { get; set; }
    public Car? Car { get; set; }

}