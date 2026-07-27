using Mover.Core.Dto.Category;
using Mover.HttpUtility;
using Mover.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mover.Core.Exceptions;
using Mover.Core.Services.Interfaces;
using Mover.Extension;
using Mover.Areas.Admin.ViewModel.Category;
using Microsoft.AspNetCore.Authorization;
using Mover.Core.Enums.Roles;
using Mover.ViewModel.Filter;
using Mover.Core.Dto.Filter;
using Microsoft.AspNetCore.Hosting;
using Mover.Core.Helpers;

namespace Mover.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IFileHelper _fileHelper;
        public CategoryController(ICategoryService categoryService, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, IFileHelper fileHelper)
        {
            _categoryService = categoryService;
            _webHostEnvironment=webHostEnvironment;
            _configuration=configuration;
            _fileHelper=fileHelper;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                var vm = categories.Select(a => new CategoryViewModel
                {
                    CreatedOn = a.CreatedOn,
                    Name = a.Name,
                    Id = a.Id,
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
        public async Task<IActionResult> LoadCategories(FilterViewModel model)
        {
            try
            {
                var dto = new FilterDto()
                {
                    Search = model.Search,
                    PageSize = model.PageSize,
                    PageIndex = model.PageIndex
                };
                var (categoryList, totalCount) = await _categoryService.GetAllCategoriesForGrid(dto);
                var datas = categoryList.Select(a => new CategoryViewModel
                {
                    CreatedOn = a.CreatedOn,
                    Name = a.Name,
                    Id = a.Id,
                }).ToList();
                var result = Json(new { data = datas, totalCount = totalCount });
                return result;

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
        public async Task<IActionResult> Create()
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
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
                new SeriLogger().Information("Hit2 method");

                var savedFileNames = new List<string>();
                var invalidFiles = new List<string>();
                var destinationFolder = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    _configuration["ImageSettings:CategoryImages"]);

                if (!Directory.Exists(destinationFolder))
                {
                    new SeriLogger().Information("Hit3 method");

                    Directory.CreateDirectory(destinationFolder);
                }

                foreach (var image in model.Images)
                {
                    new SeriLogger().Information("Hit4 method");

                    if (!_fileHelper.IsImageValid(image.FileName))
                    {
                        new SeriLogger().Information("Hit5 method");

                        invalidFiles.Add(image.FileName);
                        continue;
                    }
                    new SeriLogger().Information("Hit5 method");

                    var fileName = await _fileHelper.SaveImageAndGetFileName(image, destinationFolder);
                    new SeriLogger().Information("Hit6 method");

                    var imagePath = Path.Combine(_configuration["ImageSettings:CategoryImages"], fileName);
                    new SeriLogger().Information("Hit7 method");

                    savedFileNames.Add(imagePath);
                    new SeriLogger().Information("Hit8 method");

                }

                    var dto = new CategoryDto()
                {
                    Name = model.Name,
                    ImageUrl = savedFileNames.FirstOrDefault(),
                };
                new SeriLogger().Information("Hit9 method");

                await _categoryService.Save(dto);
                new SeriLogger().Information("Hit10 method");

                this.NotifySuccess("Sucessfully created category");
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
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var order = await _categoryService.GetCategory(id);
                var vm = new CategoryViewModel
                {
                    Id = order.Id,
                    Name = order.Name,
                    ImageUrl=order.ImageUrl
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
        public async Task<IActionResult> Edit(CategoryViewModel model)
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
                  _configuration["ImageSettings:CategoryImages"]);
                var dto = new CategoryDto()
                {
                    Id = model.Id,
                    Name = model.Name,
                    ImageUrl=model.ImageUrl,
                    Images=model.Images
                };

                await _categoryService.Edit(dto,imagePath);
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
        public async Task<IActionResult> Delete([FromForm] int categoryId)
        {
            try
            {
                await _categoryService.Delete(categoryId);
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
