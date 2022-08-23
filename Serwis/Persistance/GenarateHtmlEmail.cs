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
                html += @"<table style='border-collapse: collapse; width: 100%;' border='1'>
                            <thead>
                                <tr style='background-color: #7A7A7A; color: white'>
                                    <td style='width: 33.3333%; text-align: center;'>
                                        <h4 style='text-align: center;'>Produkt</h4>
                                    </td>
                                    <td style='width: 33.3333%; text-align: center;'>
                                        <h4>Ilość</h4>
                                    </td>
                                    <td style='width: 33.3333%; text-align: center;'>
                                        <h4>Cena</h4>
                                    </td>
                                </tr>
                            </thead>
                            <tbody>";

                foreach (var position in order.OrderPositions)
                {
                    html += $@"<tr>
                                    <td style='width: 33.3333%; text-align: center;'>{position.Product.Name}</td>
                                    <td style='width: 33.3333%; text-align: center;'>{position.Quantity}</td>
                                    <td style='width: 33.3333%; text-align: center;'>{(position.Product.Price * position.Quantity).ToString("0.00")}</td>
                            </tr>";


                }
                    html += $@"
                           <tr>
                                <td style='width: 66%; text-align: right;' colspan='2' ><strong>Cena za wszystko</strong></td>
                                <td style='width: 33.3333%; text-align: center;'><strong>{order.FullPrice.ToString("0.00")}</strong></td>
                            </tr>
                            </tbody>
                    </table>";
            }
            



            html += @"<br /> <br /> Wiadomosc wysłana z sklepu internetowego.";
            return html;
































        }
    }
}
