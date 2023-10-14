using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
     public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository 
     {
          private readonly ApplicationDbContext _db;

          public OrderHeaderRepository(ApplicationDbContext db) : base(db)
          {
               _db = db;
          }

          public void Update(OrderHeader orderHeader)
          {
               _db.OrderHeaders.Update(orderHeader);
          }

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderHeader = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
               if (orderHeader == null) return;

               orderHeader.OrderStatus = orderStatus;

               if (!String.IsNullOrEmpty(paymentStatus))
               {
                    orderHeader.PaymentStatus = paymentStatus;
               }
		}

		public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
			var orderHeader = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if (orderHeader == null) return;

               if (!String.IsNullOrEmpty(sessionId))
               {
                    orderHeader.SessionId = sessionId;
               }

			if (!String.IsNullOrEmpty(paymentIntentId))
			{
                    orderHeader.PaymentIntentId = paymentIntentId;
                    orderHeader.PaymentDate = DateTime.Now; 
			}
		}
	}
}
