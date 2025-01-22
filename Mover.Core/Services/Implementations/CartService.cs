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

namespace Mover.Core.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<List<CartDto>> GetAllCarts(int userId)
        {
            var carts = await _cartRepository.GetQueryable().Where(a => a.UserId == userId).ToListAsync();
            decimal totalPrice = carts.Sum(a => a.Quantity * (a.Product.DiscountedPrice ?? 0));
            var dto = carts.Select(a => new CartDto()
            {
                CartId = a.CartId,
                ProductId = a.ProductId,
                ProductName = a.Product.ProductName,
                Quantity = a.Quantity,
                ImageUrl = a.Product.ProductImages
                    .Where(image => image.IsMainImage == true)
                    .Select(image => image.ImageUrl).ToList(),
                ProductPrice = a.Product.DiscountedPrice,
                TotalPrice = totalPrice
            }).ToList();
            return dto;
        }

        public async Task IncreaseCount(int cartId)
        {
            using var tx = TransactionScopeHelper.GetInstance();
            var cart = await GetCartById(cartId);
            cart.Quantity += 1;
            _cartRepository.Update(cart);
            tx.Complete();

        }
        public async Task DecreaseCount(int cartId)
        {
            using var tx = TransactionScopeHelper.GetInstance();
            var cart = await GetCartById(cartId);
            if (cart.Quantity <= 1)
            {
                await _cartRepository.DeleteAsync(cart);
            }
            else
            {
                cart.Quantity -= 1;
                _cartRepository.Update(cart);
            }
            tx.Complete();

        }

        public async Task Save(CartDto model)
        {
            using var tx = TransactionScopeHelper.GetInstance();

            var entity = new ShoppingCart()
            {
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                UserId = model.CreatedBy,
                AddedAt = DateTime.Now
            };

            await _cartRepository.InsertAsync(entity);

            tx.Complete();

        }


        private async Task<ShoppingCart> GetCartById(int cartId)
        {
            return await _cartRepository.GetQueryable()
                .Where(a => a.CartId == cartId)
                .FirstOrDefaultAsync()
                ?? throw new CustomException($"No cart found with ID {cartId}.");
        }
        public async Task<SummaryDto> GetSummary(UserSessionDto currentUser)
        {
            var summary = await _cartRepository.GetQueryable().Where(a => a.UserId == currentUser.Id).ToListAsync();
            var firstCart= summary.FirstOrDefault();
            var dto = new SummaryDto()
            {
                ShippingDetails = firstCart == null
            ? null
            : new ShippingDto
            {
                CreaterName = firstCart.User.FullName,
                ShippingAddressLine = firstCart.User.Addresses?.FirstOrDefault()?.AddressLine, 
                ShippingCity = firstCart.User.Addresses?.FirstOrDefault()?.City, 
                ShippingState = firstCart.User.Addresses?.FirstOrDefault()?.State,
                ShippingZipCode = firstCart.User.Addresses?.FirstOrDefault()?.ZipCode 
            },
                CartDto = summary.Select(a => new CartDto
                {
                    CartId = a.CartId,
                    ProductId = a.ProductId,
                    ProductName = a.Product.ProductName,
                    Quantity = a.Quantity,
                    ProductPrice = a.Product.DiscountedPrice,
                    TotalPrice = a.Quantity * (a.Product.DiscountedPrice ?? 0),
                }).ToList()
            };

            return dto;
        }
    }
}
