using Mover.Core.Dto.User;
using Mover.Core.Dto.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mover.Core.Dto.Carts;
using Mover.Core.Dto.Order;
using Mover.Core.Dto.Filter;

namespace Mover.Core.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllOrders(UserSessionDto currentUser);
        Task<(List<OrderDto>, int TotalCount)> GetAllOrdersForGrid(FilterDto filter, UserSessionDto currentUser);
        Task Save(OrderDto model);
        Task UpdateOrderStatus(OrderStatusDto model);
        Task Delete(int id);

    }
}
