using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
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

          public IActionResult Details(int orderId)
          {
               OrderVM = new()
               {
                    OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                    OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
               };
               return View(OrderVM);
          }

          [HttpPost]
          [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
          public IActionResult UpdateOrderDetail()
          {
               var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
               orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
               orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
               orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
               orderHeaderFromDb.City = OrderVM.OrderHeader.City;
               orderHeaderFromDb.State = OrderVM.OrderHeader.State;
               orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
               if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
               {
                    orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
               }
               if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
               {
                    orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
               }
               _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
               _unitOfWork.Save();

               TempData["Success"] = "Order Details Updated Successfully.";


               return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
          }

          #region API CALLS

          [HttpGet]
		public IActionResult GetAll(string status)
		{
			List<OrderHeader> objOrderHeaderList;

               if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
               {
                    objOrderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
               }
               else
               {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                    objOrderHeaderList = _unitOfWork.OrderHeader
                        .GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser").ToList();
               }

               switch (status)
               {
                    case "pending":
                         objOrderHeaderList = _unitOfWork.OrderHeader.GetAll(u => u.PaymentStatus == SD.PaymentStatusPending).ToList();
                         break;
                    case "inprocess":
                         objOrderHeaderList = _unitOfWork.OrderHeader.GetAll(u => u.OrderStatus == SD.StatusInProcess).ToList();
                         break;
                    case "completed":
                         objOrderHeaderList = _unitOfWork.OrderHeader.GetAll(u => u.OrderStatus == SD.StatusShipped).ToList();
                         break;
                    case "approved":
                         objOrderHeaderList = _unitOfWork.OrderHeader.GetAll(u => u.OrderStatus == SD.StatusApproved).ToList();
                         break;
                    default:
                         break;
               }

               return Json(new { data = objOrderHeaderList });
		}

		#endregion
	}
}
