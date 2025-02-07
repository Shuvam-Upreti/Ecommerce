using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mover.Areas.Admin.ViewModel.Product;
using Mover.Core.Dto.Product;
using Mover.Core.Exceptions;
using Mover.Core.Helpers;
using Mover.Core.Services.Interfaces;
using Mover.Extension;
using Mover.HttpUtility;
using Mover.Logging;

namespace Mover.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IFileHelper _fileHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        public ProductController(IProductService productService, ICategoryService categoryService, IFileHelper fileHelper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _productService = productService;
            _categoryService = categoryService;
            _fileHelper = fileHelper;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            try
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
                    OriginalPrice = a.OriginalPrice
                }).ToList();
                return View(vm);

            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong.Please try again");
                return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                ViewBag.Categories = categories.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name,
                });
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.NotifyModelStateErrors();
                return View(model);
            }

            if (model.Images == null || !model.Images.Any())
            {
                this.NotifyInfo("Please upload at least one image.");
                return View(model);
            }

            try
            {
                var savedFileNames = new List<string>();
                var invalidFiles = new List<string>();
                var destinationFolder = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    _configuration["ImageSettings:DestinationFolder"]);

                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                foreach (var image in model.Images)
                {
                    if (!_fileHelper.IsImageValid(image.FileName))
                    {
                        invalidFiles.Add(image.FileName);
                        continue;
                    }

                    var fileName = await _fileHelper.SaveImageAndGetFileName(image, destinationFolder);
                    var imagePath = Path.Combine(_configuration["ImageSettings:DestinationFolder"], fileName);
                    savedFileNames.Add(imagePath);
                }

                var dto = new ProductDto
                {
                    ProductName = model.ProductName,
                    Description = model.Description,
                    OriginalPrice = model.OriginalPrice,
                    DiscountedPrice = model.DiscountedPrice,
                    DiscountPercentage = model.DiscountPercentage,
                    CategoryId = model.CategoryId,
                    ImageUrls = savedFileNames,
                    InStock = model.InStock
                };

                await _productService.Save(dto);

                this.NotifySuccess("Product created successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong. Please try again.");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                ViewBag.Categories = categories.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name,
                });
                var product = await _productService.GetProduct(id);
                var vm = new ProductViewModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    OriginalPrice = product.OriginalPrice,
                    DiscountedPrice = product.DiscountedPrice,
                    DiscountPercentage = product.DiscountPercentage,
                    CategoryId = product.CategoryId,
                    InStock = product.InStock,
                    // If product has images, map them to the view model
                    ImageUrls = product.ImageUrls
                };

                return View(vm);

            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyInfo(ex.Message);
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
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.NotifyModelStateErrors();
                return View(model);
            }
            try
            {
                var currentUser = SessionInfo.GetCurrentUser();
                var imagePath = Path.Combine(
                   _webHostEnvironment.WebRootPath,
                   _configuration["ImageSettings:DestinationFolder"]);
                var dto = new ProductDto()
                {
                    ProductId = model.ProductId,
                    ProductName = model.ProductName,
                    Description = model.Description,
                    OriginalPrice = model.OriginalPrice,
                    DiscountedPrice = model.DiscountedPrice,
                    InStock = model.InStock,
                    DiscountPercentage = model.DiscountPercentage,
                    CategoryId = model.CategoryId,
                    ImageUrls = model.ImageUrls, // Existing images
                    NewImages = model.Images, // New images to be added
                    DeleteImages = model.DeleteImages // Images to be deleted
                };

                await _productService.Edit(dto, imagePath);
                this.NotifySuccess("Sucessfully updated category");
                return RedirectToAction(nameof(Index));
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return View(model);
            }

            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong.Please try again");
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int productId)
        {
            try
            {
                await _productService.Delete(productId);
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

        [HttpGet]
        public async Task<IActionResult> ViewProducts([FromQuery] int? categoryId)
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                var products = categoryId is not null
                    ? await _productService.GetProductsByCategories(new List<int> { categoryId.Value })
                    : await _productService.GetAllProducts();

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
                ViewBag.SelectedCategoryId = categoryId;
                ViewBag.Categories = categories;
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
        public async Task<IActionResult> GetProductsByCategories([FromQuery] List<int> categoryIds)
        {
            if (categoryIds.Count <= 0)
            {
                return Json(new { success = false, message = "No catergory" });
            }
            try
            {
                var products = await _productService.GetProductsByCategories(categoryIds);
                var productList = products.Select(a => new
                {
                    a.ProductId,
                    a.ProductName,
                    a.Description,
                    a.DiscountedPrice,
                    a.OriginalPrice,
                    ImageUrl = a.ImageUrls.FirstOrDefault(), // Assuming one image per product
                    a.Category
                });

                return Json(productList);
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                return Json(new { success = false, message = "Something went wrong. Please try again." });
            }
        }

    }
}
