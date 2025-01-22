using Mover.Core.Dto.User;
using Mover.Core.Dto.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mover.Core.Dto.Product;

namespace Mover.Core.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProducts();
        Task Save(ProductDto model);
        Task<ProductDto> GetProduct(int id);
        Task Edit(ProductDto model,string? imagePath);
        Task Delete(int id);
    }
}
