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
            using var tx = TransactionScopeHelper.GetInstance() ;

            var entity = await _orderRepository.GetByIdAsync(model.OrderId) ?? throw new CustomException("No Order Found");

            entity.OrderStatus = model.OrderStatus;
            _orderRepository.Update(entity);

            tx.Complete();

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
