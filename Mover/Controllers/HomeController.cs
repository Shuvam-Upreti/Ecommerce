using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mover.Areas.Admin.ViewModel.Product;
using Mover.Core.Dto.Carts;
using Mover.Core.Dto.Category;
using Mover.Core.Exceptions;
using Mover.Core.Services.Implementations;
using Mover.Core.Services.Interfaces;
using Mover.Extension;
using Mover.HttpUtility;
using Mover.Logging;
using Mover.Models;
using Mover.ViewModel.Carts;
using System.Diagnostics;

namespace Mover.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProducts();
            var vm = products.Select(a => new ProductViewModel
            {
                ProductId = a.ProductId,
                Description = a.Description,
                ProductName = a.ProductName,
                DiscountedPrice = a.DiscountedPrice,
                Category = a.Category,
                DiscountPercentage = a.DiscountPercentage,
                OriginalPrice = a.OriginalPrice,
                ImageUrls = a.ImageUrls,
            }).ToList();
            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var products = await _productService.GetProduct(id);
                var vm = new CartViewModel
                {
                    ProductId = products.ProductId,
                    Description = products.Description,
                    ProductName = products.ProductName,
                    DiscountedPrice = products.DiscountedPrice,
                    Category = products.Category,
                    DiscountPercentage = products.DiscountPercentage,
                    OriginalPrice = products.OriginalPrice,
                    ImageUrls = products.ImageUrls,
                };
                return View(vm);
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong.Please try again");
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddToCart(CartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.NotifyModelStateErrors();
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var currentUser = SessionInfo.GetCurrentUser();
                var dto = new CartDto()
                {
                    ProductId = model.ProductId,
                    ProductName = model.ProductName,
                    Quantity = model.Quantity,
                    TotalPrice = model.TotalPrice,
                    CreatedBy = currentUser.Id,
                };

                await _cartService.Save(dto);
                this.NotifySuccess("Sucessfully added to cart");
                return RedirectToAction(nameof(Index));
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong.Please try again");
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
