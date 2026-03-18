public class ServiceRecord
{
    //proprerties: id, Service type, descritpion, cost, date, mileage, car id, car
    public int Id { get; set; }
    public string ServiceType { get; set; }
    public string Description { get; set; }
    public decimal Cost { get; set; }
    public DateTime Date { get; set; }
    public int Mileage { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; }

}