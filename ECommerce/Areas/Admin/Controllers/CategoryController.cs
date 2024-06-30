using ECommerce.DataAccess.Repoistory.IRepository;
using ECommerce.Models;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize( Roles = SD.Role_Admin+","+SD.Role_Employee)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var categoryList = _unitOfWork.category.GetAll();
            return Json(new { data = categoryList });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryFromDb = _unitOfWork.category.Get(id);
            if (categoryFromDb == null)
                return Json(new { success = false, message = "Something went wrong!" });
            _unitOfWork.category.Remove(categoryFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data Deleted Successfully." });
        }
        #endregion

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null) return View(category);

            category = _unitOfWork.category.Get(id.GetValueOrDefault());
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category == null) return NotFound();
            if (!ModelState.IsValid) return View();

            if (category.Id == 0)
                _unitOfWork.category.Add(category);
            else
                _unitOfWork.category.update(category);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        
    }
}