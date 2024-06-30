using ECommerce.DataAccess.Data;
using ECommerce.Models;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin +","+SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.Include(a => a.Company).ToList();      //AspNetUser
            var roles = _context.Roles.ToList();        //aspnetroles
            var userRoles = _context.UserRoles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(r => r.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = "User is not from any company."
                    };
                }
            }
            var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            userList.Remove(adminUser);

            return Json(new { data = userList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            bool isLocked = false;
            var UserInDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (UserInDb == null)
                return Json(new { success = false, message = "Something went Wrong" });
            if(UserInDb!=null && UserInDb.LockoutEnd > DateTime.Now)
            {
                UserInDb.LockoutEnd = DateTime.Now;
                isLocked = false;
            }
            else
            {
                UserInDb.LockoutEnd = DateTime.Now.AddYears(100);
                isLocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = isLocked == true ? "User successfully locked." : "User successfully unlocked." });
        }
        #endregion
    }
}
