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

        public async Task<List<OrderDto>> GetAllOrders(UserSessionDto currentUser, string? guestId)
        {
            var orders = _orderRepository.GetQueryable();
            if (currentUser==null&&guestId is not null)
            {
                orders = orders.Where(a => a.GuestId == guestId);
            }
            else
            {

                if (currentUser.Role != RolesEnum.Admin.ToString())
                {
                    orders = orders.Where(a => a.UserId == currentUser.Id);

                }
            }
            orders = orders.OrderByDescending(a => a.OrderDate);
            var dto = orders.Select(a => new OrderDto()
            {
                OrderId = a.OrderId,
                CreatedBy = a.User.FullName??a.CreaterName,
                PhoneNumber = a.User.AspUser.PhoneNumber??a.PhoneNumber,
                TotalAmount = a.TotalAmount,
                OrderDate = a.OrderDate,
                OrderStatus = a.OrderStatus,
                OrderItemsDto= a.OrderItems.Select(a => new OrderItemDto()
                {
                    ProductName=a.Product.ProductName,
                    Quantity=a.Quantity,
                    PriceAtPurchase=a.PriceAtPurchase,
                    ImageUrl=a.Product.ProductImages.FirstOrDefault().ImageUrl
                }).ToList()

            }).ToList();
            return dto;
        }
        public async Task<(List<OrderDto>, int TotalCount)> GetAllOrdersForGrid(FilterDto filter, UserSessionDto currentUser, string? orderStatus, string? guestId, string? filterDateFrom, string? filterDateTo, string? searchInput)
        {
            var orders = _orderRepository.GetQueryable();
            if (orderStatus is not null)
            {
                orders=orders.Where(a => a.OrderStatus==orderStatus);

            }

            if (currentUser==null&&guestId is not null)
            {
                orders = orders.Where(a => a.GuestId == guestId);
            }
            else
            {

                if (currentUser.Role != RolesEnum.Admin.ToString())
                {
                    orders = orders.Where(a => a.UserId == currentUser.Id);

                }
            }
            if (!string.IsNullOrEmpty(filterDateFrom) && DateTime.TryParse(filterDateFrom, out DateTime fromDate) && !string.IsNullOrEmpty(filterDateTo) && DateTime.TryParse(filterDateTo, out DateTime toDate))
            {
                orders = orders.Where(a => a.OrderDate.Value.Date >= fromDate.Date && a.OrderDate.Value.Date <= toDate.Date);
            }
            if (!string.IsNullOrEmpty(searchInput))
            {
                orders = orders.Where(a =>
                    (a.User.FullName != null && a.User.FullName.Contains(searchInput)) ||
                    (a.CreaterName != null && a.CreaterName.Contains(searchInput)) ||
                    (a.PhoneNumber != null && a.PhoneNumber.Contains(searchInput)) ||
                    (a.User.AspUser.PhoneNumber != null && a.User.AspUser.PhoneNumber.Contains(searchInput))
                );
            }

            orders = orders.OrderByDescending(a => a.OrderDate);
            int totalCount = await orders.CountAsync();
            var pagedData = orders.Skip(filter.PageIndex).Take(filter.PageSize);
            var dto = pagedData.Select(a => new OrderDto()
            {
                OrderId = a.OrderId,
                CreatedBy = a.CreaterName??a.User.FullName,
                PhoneNumber = a.PhoneNumber??a.User.AspUser.PhoneNumber,
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
                GuestId = model.GuestId,
                PhoneNumber = model.PhoneNumber,
                GuestEmail = model.GuestEmail,
                OrderDate = model.OrderDate,
                ShippingAddressLine = model.ShippingAddressLine,
                ShippingCity = model.ShippingCity,
                ShippingZipCode = model.ShippingZipCode,
                TotalAmount = model.TotalAmount,
                ShippingState = model.ShippingState,
                CreaterName = model.CreatedBy,
                OrderItems = model.OrderItemsDto.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.PriceAtPurchase,
                    DiscountAtPurchase = item.DiscountAtPurchase
                }).ToList()
            };

            await _orderRepository.InsertAsync(entity);
            var carts = await _cartRepository.GetQueryable().Where(a => a.UserId == model.UserId|| a.GuestId == model.GuestId).ToListAsync();
            if (carts.Count>0)
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
                CreatedBy = order.User?.FullName??order.CreaterName,
                PhoneNumber = order.User?.AspUser.PhoneNumber ?? order.PhoneNumber,
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
                    ProductName=a.Product?.ProductName,
                    ImageUrl = a.Product?.ProductImages?.FirstOrDefault()?.ImageUrl
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
