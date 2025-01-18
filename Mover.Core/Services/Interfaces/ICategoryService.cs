using Mover.Core.Dto.User;
using Mover.Core.Dto.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategories();
        Task Save(CategoryDto model);
        Task<CategoryDto> GetCategory(int id);
        Task Edit(CategoryDto model);
        Task Delete(int id);
    }
}
