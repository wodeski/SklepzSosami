using Serwis.Core.Models;
using Serwis.Models.Domains;
using Serwis.Models.ViewModels;

namespace Serwis.Persistance
{
    public class GenarateHtmlEmail : IGenarateHtmlEmail
    {
        public string GenerateInvoice(OrderViewModel order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order)); //tez do zmiany
            var html = $"Zamówienie {order.Title}.<br /><br />";

            if (order.OrderPositions != null && order.OrderPositions.Any())
            {
                html += @"<table border = 1 cellpadding=5 cellspacing=1>
                        <tr>
                            <td align = center bgcolor= lightgrey>Produkt</td>
                            <td align = center bgcolor= lightgrey>Ilość</td>
                            <td align = center bgcolor= lightgrey>Cena</td>
                        </tr>";
                foreach (var position in order.OrderPositions)
                {
                    html += $@"<tr>
                            <td aling = center>{position.Product.Name}</td>
                            <td align = center>{position.Quantity}</td>
                            <td align = center>{(position.Product.Price * position.Quantity).ToString("0.00")}</td>
                            </tr>";


                }
                html += $@"<tr>
                            <td colspan='2' aling = center>Cena za wszystko</td>
                            <td aling = center>{order.FullPrice.ToString("0.00")}</td>
                            </tr> 
               </table>";
            }
            else
                html += " --brak widomości do wyświetlenia";



            html += @"<br /> <br /> Wiadomosc wysłana z sklepu internetowego.";
            return html;
        }
    }
}
