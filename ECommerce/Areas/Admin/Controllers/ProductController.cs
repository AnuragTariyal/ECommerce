using ECommerce.DataAccess.Repoistory.IRepository;
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _uniOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment)
        {
            _uniOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _uniOfWork.Product.GetAll() });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productInDb = _uniOfWork.Product.Get(id);
            if (productInDb == null)
                return Json(new { success = false, message = "Something went wrong!" });
            var webRootPath = _webHostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, productInDb.ImageUrl.Trim('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _uniOfWork.Product.Remove(productInDb);
            _uniOfWork.Save();
            return Json(new { success = true, message = "Data Deleted successfully." });
        }
        #endregion

        public IActionResult Upsert(int? id)
        {
            ProductVm productVm = new ProductVm()
            {
                Product = new Product(),
                CategoryList = _uniOfWork.category.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString()
                }),
                CoverTypeList = _uniOfWork.coverType.GetAll().Select(cl => new SelectListItem()
                {
                    Text=cl.Name,
                    Value=cl.Id.ToString()
                })
            };
            if (id == null) return View(productVm);
            productVm.Product = _uniOfWork.Product.Get(id.GetValueOrDefault());
            return View(productVm);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(ProductVm productVm)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var uploads = Path.Combine(webRootPath, @"Images\Products");
                    if (productVm.Product.Id != 0)
                    {
                        var imageExists = _uniOfWork.Product.Get(productVm.Product.Id).ImageUrl;
                        productVm.Product.ImageUrl = imageExists;
                    }
                    if (productVm.Product.ImageUrl != null)
                    {
                        var imagePath = Path.Combine(webRootPath, productVm.Product.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using(var fileStream=new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVm.Product.ImageUrl = @"\Images\Products\" + fileName + extension;
                }
                else
                {
                    if (productVm.Product.Id != 0)
                    {
                        var imageExists = _uniOfWork.Product.Get(productVm.Product.Id).ImageUrl;
                        productVm.Product.ImageUrl = imageExists;
                    }
                }
                if (productVm.Product.Id == 0)          ///if error occur use{}
                    _uniOfWork.Product.Add(productVm.Product);
                else
                    _uniOfWork.Product.Update(productVm.Product);
                _uniOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVm = new ProductVm()
                {
                    Product = new Product(),
                    CategoryList = _uniOfWork.category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                    CoverTypeList = _uniOfWork.coverType.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    })
                };
                if(productVm.Product.Id!=0)
                {
                    productVm.Product = _uniOfWork.Product.Get(productVm.Product.Id);
                }
                return View(productVm);
            }
        }
    }
}
