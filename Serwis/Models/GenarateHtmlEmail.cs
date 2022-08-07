namespace Serwis.Models
{
    public class GenarateHtmlEmail
    {
        public string GenerateInvoice(Report report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));
            var html = $"Raport{report.Title} z dnia {report.Date.ToString("dd-mm-yyyy")}.<br /><br />";

            if (report.Positions != null && report.Positions.Any())
            {
                html += @"<table border = 1 cellpadding=5 cellspacing=1>
                        <tr>
                            <td align = center bgcolor= lightgrey>Tytuł</td>
                            <td align = center bgcolor= lightgrey>Opis</td>
                            <td align = center bgcolor= lightgrey>Wartość</td>
                        </tr>";
                foreach (var position in report.Positions)
                {
                    html += $@"<tr>
                            <td aling = center>{position.Title}</td>
                            <td align = center>{position.Description}</td>
                            <td align = center>{position.Value.ToString("0.00")} zł</td>
                            </tr>";


                }
                html += "</table>";
            }
            else
                html += " --brak widomości do wyświetlenia";



            html += @"<br /> <br /> Wiadomosc wysłana z aplikacji ReportService.";
            return html;
        }
    }
}
