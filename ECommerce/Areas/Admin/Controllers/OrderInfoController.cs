using ECommerce.DataAccess.Repoistory.IRepository;
using ECommerce.Models;
using ECommerce.Utility;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderInfoController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public OrderInfoController(IUnitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Approved()
        {
            return View();
        }
        public IActionResult Pending()
        {
            return View();
        }
        public IActionResult Detail(int id)
        {
            var info = _unitofwork.OrderDetail.FirstOrDefault(u => u.OrderHeaderId == id, includeProperties: "Product,OrderHeader");
            info.OrderHeader.OrderDate.ToShortDateString();
            return View(info);
        }

        #region APIs
        public IActionResult GetAll()
        {
            return Json(new { data = _unitofwork.OrderHeader.GetAll() });
        }
        public IActionResult App()
        {
            var approved = _unitofwork.OrderHeader.GetAll(u => u.OrderStatus == SD.OrderStatusApproved);
            return Json(new { data = approved});
        }
        public IActionResult Pen()
        {
            var pending = _unitofwork.OrderHeader.GetAll(u => u.OrderStatus == SD.OrderStatusPending);
            return Json(new { data = pending });
        }
        #endregion
    }
}
