using Dapper;
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
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
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
            return Json(new { data = _unitOfWork.SP_CALL.List<CoverType>(SD.Proc_GetCoverTypes)});
            //return Json(new {data=_unitOfWork.coverType.GetAll()});
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            //var coverInDb = _unitOfWork.coverType.Get(id);
            var param = new DynamicParameters();
            param.Add("@id", id);
            var coverInDb = _unitOfWork.SP_CALL.OneRecord<CoverType>(SD.Proc_GetCoverType, param);
            if (coverInDb == null)
                return Json(new { success = false, message = "Something went wrong!" });

            _unitOfWork.SP_CALL.Execute(SD.Proc_DeleteCoverTypes, param);
            //_unitOfWork.coverType.Remove(coverInDb);
            //_unitOfWork.Save();
            return Json(new {success=true, message = "Data deleted successfully." });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null) return View(coverType);

            var param = new DynamicParameters();
            param.Add("@id", id.GetValueOrDefault());
            coverType = _unitOfWork.SP_CALL.OneRecord<CoverType>(SD.Proc_GetCoverType, param);
            //coverType = _unitOfWork.coverType.Get(id.GetValueOrDefault());
            if (coverType == null) return NotFound();
            return View(coverType);
        }
        [HttpPost]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null) return NotFound();
            if (!ModelState.IsValid) return NotFound();
            var param = new DynamicParameters();
            param.Add("@name", coverType.Name);
            if (coverType.Id == 0)
            {
                //_unitOfWork.coverType.Add(coverType);
                _unitOfWork.SP_CALL.Execute(SD.Proc_CreateCoverTypes, param);
            }
            else
            {
                param.Add("@id", coverType.Id);
                _unitOfWork.SP_CALL.Execute(SD.Proc_UpdateCoverTypes, param);

            }//_unitOfWork.coverType.Update(coverType);
                //_unitOfWork.Save();

                return RedirectToAction("Index");
        }
    }
}
