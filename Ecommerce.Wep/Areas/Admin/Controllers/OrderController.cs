using Ecommerce.DataAccess.Implementation;
using Ecommerce.Models.Models;
using Ecommerce.Models.Repository;
using Ecommerce.Models.ViewModels;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Ecommerce.Wep.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public OrderVM OrderVM { get; set; }

		public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetData()
        {
            IEnumerable<OrderHeader> orders = _unitOfWork.OrderHeader.GetAll(includedWord: "ApplicationUser");
            return Json(new {data = orders});
        }


        public IActionResult Details(int orderid)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetItem(u => u.Id == orderid, includedWord: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderid, includedWord: "Product")
            };

            return View(orderVM);
        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetails()
		{
			var orderFromDb = _unitOfWork.OrderHeader.GetItem(u => u.Id == OrderVM.OrderHeader.Id);
			orderFromDb.Name = OrderVM.OrderHeader.Name;
			orderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			orderFromDb.Address = OrderVM.OrderHeader.Address;
			orderFromDb.City = OrderVM.OrderHeader.City;

			if (OrderVM.OrderHeader.Carrier != null)
			{
				orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
			}

			if (OrderVM.OrderHeader.TrakcingNumber != null)
			{
				orderFromDb.TrakcingNumber = OrderVM.OrderHeader.TrakcingNumber;
			}

			_unitOfWork.OrderHeader.Update(orderFromDb);
			_unitOfWork.Complete();
			TempData["Update"] = "Item has Updated Successfully";
			return RedirectToAction("Index", "Order");
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartProcess()
		{
			_unitOfWork.OrderHeader.UpdateOrderStatus(OrderVM.OrderHeader.Id, SD.Proccessing, null);
			_unitOfWork.Complete();
			TempData["Update"] = "Order Status has Updated Successfully";
			return RedirectToAction("Details", "Order", new {orderid = OrderVM.OrderHeader.Id });
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartShip()
		{
			var orderFromDb = _unitOfWork.OrderHeader.GetItem(u => u.Id == OrderVM.OrderHeader.Id);
			orderFromDb.TrakcingNumber = OrderVM.OrderHeader.TrakcingNumber;
			orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
			orderFromDb.OrderStatus = SD.Shipped;
			orderFromDb.ShippingDate = DateTime.Now;

			_unitOfWork.OrderHeader.Update(orderFromDb);
			_unitOfWork.Complete();

			TempData["Update"] = "Order has Shipped Successfully";
			return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var orderFromDb = _unitOfWork.OrderHeader.GetItem(u => u.Id == OrderVM.OrderHeader.Id);
			if (orderFromDb.PaymentStatus == SD.Approve)
			{
				var option = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderFromDb.PaymentIntentId
				};

				var service = new RefundService();
				Refund refund = service.Create(option);

				_unitOfWork.OrderHeader.UpdateOrderStatus(orderFromDb.Id, SD.Cancelled, SD.Refund);
			}
			else
			{
				_unitOfWork.OrderHeader.UpdateOrderStatus(orderFromDb.Id, SD.Cancelled, SD.Cancelled);
			}
			_unitOfWork.Complete();

			TempData["Update"] = "Order has Cancelled Successfully";
			return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
		}
	}
}
