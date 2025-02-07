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

namespace Mover.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
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
            try
            {
                var dto = new CategoryDto()
                {
                    Name = model.Name
                };

                await _categoryService.Save(dto);
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
                var dto = new CategoryDto()
                {
                    Id = model.Id,
                    Name = model.Name
                };

                await _categoryService.Edit(dto);
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.Delete(id);
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
