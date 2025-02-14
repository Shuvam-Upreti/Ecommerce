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
using Mover.Core.Dto.Carts;
using Mover.Core.Dto.Order;
using Mover.Core.Enums.Roles;
using Mover.Core.Dto.Filter;
using Mover.Core.Dto.Product;

namespace Mover.Core.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
        }

        public async Task<List<OrderDto>> GetAllOrders(UserSessionDto currentUser)
        {
            var orders = _orderRepository.GetQueryable();
            if (currentUser.Role != RolesEnum.Admin.ToString())
            {
                orders = orders.Where(a => a.UserId == currentUser.Id);

            }
            orders = orders.OrderByDescending(a => a.OrderDate);
            var dto = orders.Select(a => new OrderDto()
            {
                OrderId = a.OrderId,
                CreatedBy = a.User.FullName,
                PhoneNumber = a.User.AspUser.PhoneNumber,
                TotalAmount = a.TotalAmount,
                OrderDate = a.OrderDate,
                OrderStatus = a.OrderStatus,
            }).ToList();
            return dto;
        }
        public async Task<(List<OrderDto>, int TotalCount)> GetAllOrdersForGrid(FilterDto filter, UserSessionDto currentUser)
        {
            var orders = _orderRepository.GetQueryable();
            if (currentUser.Role != RolesEnum.Admin.ToString())
            {
                orders = orders.Where(a => a.UserId == currentUser.Id);

            }
            orders = orders.OrderByDescending(a => a.OrderDate);
            int totalCount = await orders.CountAsync();
            var pagedData = orders.Skip(filter.PageIndex).Take(filter.PageSize);
            var dto = pagedData.Select(a => new OrderDto()
            {
                OrderId = a.OrderId,
                CreatedBy = a.User.FullName,
                PhoneNumber = a.User.AspUser.PhoneNumber,
                TotalAmount = a.TotalAmount,
                OrderDate = a.OrderDate,
                OrderStatus = a.OrderStatus,
            }).ToList();
            return (dto, totalCount);
        }
        public async Task Save(OrderDto model)
        {
            using var tx = TransactionScopeHelper.GetInstance();

            var entity = new Order
            {
                UserId = model.UserId,
                OrderDate = model.OrderDate,
                ShippingAddressLine = model.ShippingAddressLine,
                ShippingCity = model.ShippingCity,
                ShippingZipCode = model.ShippingZipCode,
                TotalAmount = model.TotalAmount,
                ShippingState = model.ShippingState,
                OrderItems = model.OrderItemsDto.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.PriceAtPurchase,
                    DiscountAtPurchase = item.DiscountAtPurchase
                }).ToList()
            };

            await _orderRepository.InsertAsync(entity);
            var carts = await _cartRepository.GetQueryable().Where(a => a.UserId == model.UserId).ToListAsync();
            _cartRepository.DeleteRange(carts);
            tx.Complete();

        }
        public async Task UpdateOrderStatus(OrderStatusDto model)
        {
            using var tx = TransactionScopeHelper.GetInstance();

            var entity = await _orderRepository.GetByIdAsync(model.OrderId) ?? throw new CustomException("No Order Found");

            entity.OrderStatus = model.OrderStatus;
            _orderRepository.Update(entity);

            tx.Complete();

        }
        public async Task<OrderDto> GetOrderById(int orderId)
        {
            var order = await _orderRepository.GetQueryable().Where(a => a.OrderId == orderId).FirstOrDefaultAsync() ?? throw new CustomException("No order found.");

            var dto = new OrderDto()
            {
                OrderId = order.OrderId,
                CreatedBy = order.User?.FullName,
                PhoneNumber = order.User?.AspUser.PhoneNumber,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                ShippingAddressLine = order.ShippingAddressLine,
                ShippingCity = order.ShippingCity,
                ShippingZipCode = order.ShippingZipCode,
                PaymentStatus = order.PaymentStatus,
                ShippingState = order.ShippingState,
                OrderItemsDto= order.OrderItems.Select(a => new OrderItemDto()
                {
                    DiscountAtPurchase = a.DiscountAtPurchase,
                    PriceAtPurchase = a.PriceAtPurchase,
                    Quantity = a.Quantity,
                    OrderItemId=a.OrderItemId,
                    ProductId=a.ProductId,
                    ProductName=a.Product?.ProductName
                }).ToList()
            };
            return dto;
        }

        public async Task Delete(int id)
        {
            using var tx = TransactionScopeHelper.GetInstance();
            var order = await _orderRepository.GetByIdAsync(id) ?? throw new CustomException("No Order Found");

            await _orderRepository.DeleteAsync(order);

            tx.Complete();
        }
    }
}
