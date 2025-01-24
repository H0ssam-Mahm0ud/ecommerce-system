using Ecommerce.DataAccess.Implementation;
using Ecommerce.Models.Models;
using Ecommerce.Models.Repository;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace Ecommerce.Wep.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int ? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = 8;

            var products = _unitOfWork.Product.GetAll().ToPagedList(pageNumber, pageSize);
            return View(products);
        }


        public IActionResult Details(int ProductId)
        {
            ShoppingCart product = new ShoppingCart()
            {
                ProductId = ProductId,
                Product = _unitOfWork.Product.GetItem(x => x.Id == ProductId, includedWord: "Category"),
                Count = 1
            };

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartObj = _unitOfWork.ShoppingCart.GetItem(
                u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

            if (cartObj == null) 
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Complete();
                HttpContext.Session.SetInt32(SD.SessionKey,
                        _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count());
            }
            else
            {
                _unitOfWork.ShoppingCart.IncreaseCount(cartObj, shoppingCart.Count);
                _unitOfWork.Complete();
            }

            return RedirectToAction("Index");

        }

    }
}
