using Ecommerce.DataAccess.Data;
using Ecommerce.Models.Models;
using Ecommerce.Models.Repository;

namespace Ecommerce.DataAccess.Implementation
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }

        public void UpdateOrderStatus(int id, string? OrderStatus, string? PaymentStatus)
        {
            var orderFromDb = _context.OrderHeaders.SingleOrDefault(x => x.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = OrderStatus;
				orderFromDb.PaymentDate = DateTime.Now;
				if (PaymentStatus != null)
                {
                    orderFromDb.PaymentStatus = PaymentStatus;
                }
            }
        }
    }
}
