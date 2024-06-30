using ECommerce.DataAccess.Repoistory.IRepository;
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private static bool isEmailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }
            else
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = _unitOfWork.ShoppingCart.GetAll(sp => sp.ApplicationUserId == claim.Value,
                    includeProperties: "Product"),
                    OrderHeader = new OrderHeader()
                };
                ShoppingCartVM.OrderHeader.OrderTotal = 0;
                ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault
                    (u => u.Id == claim.Value, includeProperties: "Company");

                foreach (var list in ShoppingCartVM.ListCart)
                {
                    list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50,
                        list.Product.Price100);

                    ShoppingCartVM.OrderHeader.OrderTotal += (list.Count * list.Price);
                    if (list.Product.Description.Length > 50)
                    {
                        list.Product.Description = list.Product.Description.Substring(0, 49) + "..show more";
                    }
                }
            }
            return View(ShoppingCartVM);
        }
        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(u => u.Id == id);
            cart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(u => u.Id == id);
            if (cart.Count == 1)
            {
                cart.Count = 1;
            }
            else
            {
                cart.Count -= 1;
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(u => u.Id == id);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            //session
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sp => sp.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value
            , includeProperties: "Company");

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price,
                    list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                list.Product.Description = SD.ConvertToRawHtml(list.Product.Description);
            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            //email confirm
            if (!isEmailConfirm)
            {
                ViewBag.EmailMessage = "Kindly verify your email !";
                ViewBag.EmailCSS = "text-primary";
            }
            else
            {
                ViewBag.EmailMessage = "Email must be confirm for authorize customer.";
                ViewBag.EmailCSS = "text-primary";
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var email = _unitOfWork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value);

            if (email.EmailConfirmed == false)
            {
                #region Email
                var user = _unitOfWork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email is empty !");
                }
                else
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    isEmailConfirm = true;
                }
                #endregion
                return RedirectToAction(nameof(Summary));
            }
            else
            {
                ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.
                    FirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");
                ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll
                    (sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product");

                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
                ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
                ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
                _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
                _unitOfWork.Save();

                foreach (var list in ShoppingCartVM.ListCart)
                {
                    list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        ProductId = list.ProductId,
                        OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                        Price = list.Price,
                        Count = list.Count
                    };
                    ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                    _unitOfWork.OrderDetail.Add(orderDetail);
                    _unitOfWork.Save();
                }
                _unitOfWork.ShoppingCart.RemoveRamge(ShoppingCartVM.ListCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, 0);
                #region Stripe
                if (stripeToken == null)
                {
                    ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(15);
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelay;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                }
                else
                {
                    //payment process
                    var options = new ChargeCreateOptions()       //stripe class
                    {
                        Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                        Currency = "usd",
                        Description = "OrderId:" + ShoppingCartVM.OrderHeader.Id,
                        Source = stripeToken
                    };
                    //payment
                    var service = new ChargeService();
                    Charge charge = service.Create(options);
                    if (charge.BalanceTransactionId == null)
                        ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                    else
                        ShoppingCartVM.OrderHeader.TransectionId = SD.PaymentStatusApproved;
                    if (charge.Status.ToLower() == "succeeded")
                    {
                        ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                        ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                        ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
                    }
                    _unitOfWork.Save();
                }
                #endregion

                return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
            }

        }
        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
    }
}