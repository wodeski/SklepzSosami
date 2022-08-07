namespace Serwis.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public bool IsSend { get; set; }
        public List<ReportPosition> Positions { get; set; }
    }
}