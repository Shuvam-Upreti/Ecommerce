using Mover.Core.Dto.User;
using Mover.Core.Dto.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mover.Core.Dto.Carts;
using Mover.Core.Dto.Order;

namespace Mover.Core.Services.Interfaces
{
    public interface ICartService
    {
        Task<List<CartDto>> GetAllCarts(int userId);
        Task Save(CartDto model);
        Task IncreaseCount(int cartId);
        Task DecreaseCount(int cartId);
        Task<SummaryDto> GetSummary(UserSessionDto currentUser);
    }
}
