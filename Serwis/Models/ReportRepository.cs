namespace Serwis.Models
{
    public class ReportRepository
    {
        public Report GetLastNotSendRepository()
        {
            //pobieranie z BD
            return new Report
            {
                Id = 1,
                Title = "R/1/2022",
                Date = new DateTime(2022, 1, 1, 1, 12, 0, 0),
                Positions = new List<ReportPosition>
                {
                    new ReportPosition
                    {
                        Id = 1,
                        ReportId = 1,
                        Title = "Position 1",
                        Description = "Descritpion 1",
                        Value = 4.002M
                    },
                     new ReportPosition
                    {
                        Id = 2,
                        ReportId = 2,
                        Title = "Position 2",
                        Description = "Descritpion 2",
                        Value = 1.1232132M
                    },
                      new ReportPosition
                    {
                        Id = 3,
                        ReportId = 3,
                        Title = "Position 3",
                        Description = "Descritpion 3",
                        Value = 21.012312302M
                    }
                }

            };
        }
        public void ReportSent(Report report)
        {
            report.IsSend = true;
            //zapis w bazie danych
        }
    }
}
