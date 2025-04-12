using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mover.Areas.Admin.ViewModel.Product;
using Mover.Core.Dto.Appsetting;
using Mover.Core.Dto.Carts;
using Mover.Core.Dto.Category;
using Mover.Core.Enums.Appsetting;
using Mover.Core.Exceptions;
using Mover.Core.Services.Implementations;
using Mover.Core.Services.Interfaces;
using Mover.Extension;
using Mover.HttpUtility;
using Mover.Logging;
using Mover.Models;
using Mover.ViewModel.Appsetting;
using Mover.ViewModel.Banner;
using Mover.ViewModel.Carts;
using System.Diagnostics;

namespace Mover.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICategoryService _categoryService;
        private readonly IAppsettingsService _appsettingService;
        private readonly GetGuestIdOrSessionUser _userHelper;
        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService, ICategoryService categoryService, GetGuestIdOrSessionUser userHelper, IAppsettingsService appsettingService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
            _categoryService=categoryService;
            _userHelper=userHelper;
            _appsettingService=appsettingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                ViewBag.Categories = categories;
                var banner = await _appsettingService.GetAppsettingByKey(AppsettingEnum.BannerImage.ToString());
                ViewBag.Banner = (banner != null && banner.Any())
                         ? banner.Select(a => a.Value).ToList(): new List<string>();
                var products = await _productService.GetAllProducts();

                var aboutUs = await _appsettingService.GetAppsettingByKey(AppsettingEnum.AboutUs.ToString());
                ViewBag.AboutUs = aboutUs != null && aboutUs.Any() ? aboutUs.FirstOrDefault().Value : string.Empty;

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
                    InStock=a.InStock
                }).ToList();
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
    
        public async Task<IActionResult> AddToCart(CartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.NotifyModelStateErrors();
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var (guestId, currentUser) = await _userHelper.GetGuestIdOrSessionUserId();

                var dto = new CartDto()
                {
                    ProductId = model.ProductId,
                    ProductName = model.ProductName,
                    Quantity = model.Quantity,
                    TotalPrice = model.TotalPrice,
                    CreatedBy = currentUser?.Id??null,
                    GuestId = guestId??null,
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
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> TestDashboard()
        {
            try
            {

                return View();
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

        [HttpGet]
        public async Task<IActionResult> About()
        {
            try
            {
                var about = await _appsettingService.GetAppsettingByPage(AppsettingEnum.AboutUsPage.ToString());
                var viewModel = await FilterByPage(about);
                return View(viewModel);
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
        private async Task<AboutUsViewModel> FilterByPage(List<AppsettingDto> dto)
        {
            var aboutUsViewModel = new AboutUsViewModel();

            // Filter and map the data from AppsettingDto to AboutUsViewModel using Enum values
            aboutUsViewModel.Title = dto.FirstOrDefault(x => x.Key == AppsettingEnum.AboutUs.ToString())?.Value ?? string.Empty;
            aboutUsViewModel.AboutUs = dto.FirstOrDefault(x => x.Key == AppsettingEnum.AboutUs.ToString())?.Value ?? string.Empty;
            aboutUsViewModel.HappyClients = dto.FirstOrDefault(x => x.Key == AppsettingEnum.HappyClients.ToString())?.Value ?? string.Empty;
            aboutUsViewModel.ProductCount = dto.FirstOrDefault(x => x.Key == AppsettingEnum.ProductCount.ToString())?.Value ?? string.Empty;
            aboutUsViewModel.DiscountPercentage = dto.FirstOrDefault(x => x.Key == AppsettingEnum.DiscountPercentage.ToString())?.Value ?? string.Empty;
            aboutUsViewModel.SupportAvailability = dto.FirstOrDefault(x => x.Key == AppsettingEnum.SupportAvailability.ToString())?.Value ?? string.Empty;
            aboutUsViewModel.CoreValueInnovation = dto.FirstOrDefault(x => x.Key == AppsettingEnum.CoreValueInnovation.ToString())?.Value ?? string.Empty;
            aboutUsViewModel.CoreValueQuality = dto.FirstOrDefault(x => x.Key == AppsettingEnum.CoreValueQuality.ToString())?.Value ?? string.Empty;
            aboutUsViewModel.CoreValueCare = dto.FirstOrDefault(x => x.Key == AppsettingEnum.CoreValueCare.ToString())?.Value ?? string.Empty;

            return aboutUsViewModel;
        }

        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            try
            {

                return View();
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
        public async Task<IActionResult> AddBanner(BannerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.NotifyModelStateErrors();
                return View(model);
            }
            try
            {
                var dto = new BannerDto()
                {
                    ImageUrl = model.ImageUrl,
                    Image=model.Image,
                };
                await _appsettingService.SaveBanner(dto);
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
    }
}
