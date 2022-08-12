using Serwis.Models.Domains;

namespace Serwis.Models
{
    public class GenarateHtmlEmail
    {
        public string GenerateInvoice(IEnumerable<OrderPosition> orderPositions, string orderTitle)
        {
            if (orderPositions == null)
                throw new ArgumentNullException(nameof(orderPositions)); //tez do zmiany
            var html = $"Zamówienie {orderTitle} z dnia {DateTime.Now.ToString("dd-MM-yyyy")}.<br /><br />";

            if (orderPositions != null && orderPositions.Any())
            {
                html += @"<table border = 1 cellpadding=5 cellspacing=1>
                        <tr>
                            <td align = center bgcolor= lightgrey>Produkt</td>
                            <td align = center bgcolor= lightgrey>Cena</td>
                        </tr>";
                foreach (var position in orderPositions)
                {
                    html += $@"<tr>
                            <td aling = center>{position.Product.Name}</td>
                            <td align = center>{position.Product.Price.ToString("0.00")}</td>
                            </tr>";


                }
                html += "</table>";
            }
            else
                html += " --brak widomości do wyświetlenia";



            html += @"<br /> <br /> Wiadomosc wysłana z sklepu internetowego.";
            return html;
        }
    }
}
