using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
     [Authorize(Roles = SD.Role_Admin)]
     public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll(string status)
		{
			List<OrderHeader> objOrderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();


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
