using Mover.Core.Dto.User;
using Mover.Core.Entities;
using Mover.Core.Entities.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mover.Core.Dto.Category;
using Mover.Core.Helpers;
using Mover.Core.Repository.Interfaces;
using Mover.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mover.Core.Exceptions;

namespace Mover.Core.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetQueryable().ToListAsync();
            var dto = categories.Select(a => new CategoryDto()
            {
                CreatedOn = a.CreatedOn,
                Name = a.Name,
                Id = a.Id,
            }).ToList();
            return dto;
        }
        public async Task Save(CategoryDto model)
        {
            using var tx = TransactionScopeHelper.GetInstance();

            var entity = new Category()
            {
                Name = model.Name,
                CreatedOn = DateTime.Now,
            };

            await _categoryRepository.InsertAsync(entity);

            tx.Complete();

        }
        public async Task<CategoryDto> GetCategory(int id)
        {
            var categories = await _categoryRepository.GetByIdAsync(id);
            var dto = new CategoryDto()
            {
                Id= categories.Id,
                Name = categories.Name,
                CreatedOn = categories.CreatedOn
            };
            return dto;
        }
        public async Task Edit(CategoryDto model)
        {
            using var tx = TransactionScopeHelper.GetInstance();
            var category = await _categoryRepository.GetByIdAsync(model.Id) ?? throw new CustomException("No Category Found");

            category.Name = model.Name;
            category.CreatedOn = DateTime.Now;
         
            _categoryRepository.Update(category);

            tx.Complete();

        }
        public async Task Delete(int id)
        {
            using var tx = TransactionScopeHelper.GetInstance();
            var category = await _categoryRepository.GetByIdAsync(id) ?? throw new CustomException("No Category Found");

            await _categoryRepository.DeleteAsync(category);

            tx.Complete();
        }
    }
}
