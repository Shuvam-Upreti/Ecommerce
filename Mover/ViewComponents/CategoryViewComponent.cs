using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mover.Core.Services.Interfaces;

namespace Mover.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;
        public CategoryViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryService.GetAllCategories();
            return View("_Category", categories);
        }
    }
}
