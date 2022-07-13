namespace Serwis.Models.Domains
{
    public class Service
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal CostOfItems { get; set; }
        public string ServiceOperations { get; set; }   
        public DateTime DateOfRealization { get; set; }
    }
}
