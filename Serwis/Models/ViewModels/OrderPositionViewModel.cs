using Serwis.Models.Domains;

namespace Serwis.Models.ViewModels
{
    public class OrderPositionViewModel
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }
        public ApplicationUser User { get; set; }
        public Order Order { get; set; }

        public Product Product { get; set; }
    }
}
