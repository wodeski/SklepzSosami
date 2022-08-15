using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwis.Models.Domains
{
    public class OrderPosition
    {
        private int _quantity = 1;
        [Key]
        public int Id { get; set; }
        public int Quantity { get;set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public ApplicationUser User { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
