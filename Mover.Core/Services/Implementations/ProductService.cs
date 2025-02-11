using Mover.Core.Dto.User;
using Mover.Core.Entities;
using Mover.Core.Entities.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mover.Core.Dto.Product;
using Mover.Core.Helpers;
using Mover.Core.Repository.Interfaces;
using Mover.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mover.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Mover.Core.Dto.Filter;
using Mover.Core.Dto.Category;

namespace Mover.Core.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly FileHelper _fileHelper;
        private readonly IConfiguration _configuration;
        public ProductService(IProductRepository productRepository, FileHelper fileHelper, IConfiguration configuration)
        {
            _productRepository = productRepository;
            _fileHelper = fileHelper;
            _configuration = configuration;
        }

        public async Task<List<ProductDto>> GetAllProducts()
        {
            var products = await _productRepository.GetQueryable().ToListAsync();
            var dto = products.Select(a => new ProductDto()
            {
                ProductId = a.ProductId,
                ProductName = a.ProductName,
                Description = a.Description,
                DiscountedPrice = a.DiscountedPrice,
                DiscountPercentage = a.DiscountPercentage,
                OriginalPrice = a.OriginalPrice,
                Category = a.Category?.Name,
                ImageUrls = a.ProductImages?.Where(img => img.IsMainImage == true)
                        .Select(img => img.ImageUrl).ToList()
            }).ToList();
            return dto;
        }
        public async Task<(List<ProductDto>, int TotalCount)> GetAllProductsForGrid(FilterDto filter)
        {
            var products = await _productRepository.GetQueryable().ToListAsync();
            int totalCount = products.Count;
            var pagedData = products.Skip(filter.PageIndex).Take(filter.PageSize);
            var dto = pagedData.Select(a => new ProductDto()
            {
                ProductId = a.ProductId,
                Category = a.Category?.Name,
                ProductName = a.ProductName,
                Description = a.Description,
                DiscountedPrice = a.DiscountedPrice,
                DiscountPercentage = a.DiscountPercentage,
                OriginalPrice = a.OriginalPrice
            }).ToList();
            return (dto, totalCount);
        }
        public async Task Save(ProductDto model)
        {
            using var tx = TransactionScopeHelper.GetInstance();

            var product = new Product
            {
                ProductName = model.ProductName,
                Description = model.Description,
                OriginalPrice = model.OriginalPrice,
                DiscountedPrice = model.DiscountedPrice,
                DiscountPercentage = model.DiscountPercentage,
                CategoryId = model.CategoryId
            };

            // Add images to the ProductImages collection
            bool isFirstImage = true;
            foreach (var imageUrl in model.ImageUrls)
            {
                product.ProductImages.Add(new ProductImage
                {
                    ImageUrl = imageUrl,
                    IsMainImage = isFirstImage
                });
                isFirstImage = false;
            }
            product.Inventories.Add(new Inventory
            {
                QuantityInStock = model.InStock,
                LastStockUpdate = DateTime.Now
            });

            await _productRepository.InsertAsync(product);

            tx.Complete();

        }
        public async Task<ProductDto> GetProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            var dto = new ProductDto()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                OriginalPrice = product.OriginalPrice,
                DiscountedPrice = product.DiscountedPrice,
                DiscountPercentage = product.DiscountPercentage,
                CategoryId = product.CategoryId,
                InStock = product.Inventories.Select(a => a.QuantityInStock).FirstOrDefault(),
                ImageUrls = product.ProductImages.Select(img => img.ImageUrl).ToList()
            };
            return dto;
        }
        public async Task Edit(ProductDto productDto, string? imagePath)
        {
            using var tx = TransactionScopeHelper.GetInstance();
            var toSaveImagePaths = _configuration["ImageSettings:DestinationFolder"];
            var product = await _productRepository.GetByIdAsync(productDto.ProductId) ?? throw new CustomException("No Category Found");
            var inventories = product.Inventories?.FirstOrDefault();
            if (productDto.DeleteImages != null && productDto.DeleteImages.Any())
            {
                foreach (var imageUrl in productDto.DeleteImages)
                {
                    // Check if the image exists in the Product's Images collection
                    var imageToDelete = product.ProductImages.FirstOrDefault(img => img.ImageUrl == imageUrl);
                    if (imageToDelete != null)
                    {
                        // Delete image from server (assuming DeleteImageAsync handles server-side deletion)
                        await _fileHelper.DeleteImageAsync(imageUrl, imagePath);

                        // Remove the image from the Product's Images collection
                        product.ProductImages.Remove(imageToDelete);
                    }
                }
            }


            if (productDto.NewImages != null && productDto.NewImages.Any())
            {
                foreach (var image in productDto.NewImages)
                {
                    var imageUrl = await _fileHelper.SaveImageAndGetFileName(image, imagePath); // Save the new image and get the URL
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = Path.Combine(toSaveImagePaths, imageUrl),
                        IsMainImage = false
                    });
                }
            }


            // Update other product properties
            product.ProductName = productDto.ProductName;
            product.CategoryId = productDto.CategoryId;
            product.Description = productDto.Description;
            product.OriginalPrice = productDto.OriginalPrice;
            product.DiscountedPrice = productDto.DiscountedPrice;
            product.DiscountPercentage = productDto.DiscountPercentage;
            if (inventories is not null)
            {
                inventories.QuantityInStock = productDto.InStock;
                inventories.LastStockUpdate = DateTime.Now;
            }
            _productRepository.Update(product);

            tx.Complete();

        }
        public async Task Delete(int id)
        {
            using var tx = TransactionScopeHelper.GetInstance();
            var product = await _productRepository.GetByIdAsync(id) ?? throw new CustomException("No Category Found");

            await _productRepository.DeleteAsync(product);

            tx.Complete();
        }
        public async Task<List<ProductDto>> GetProductsByCategories(List<int> categoryIds)
        {
            var products = await _productRepository.GetQueryable()
                .Where(p => p.CategoryId.HasValue && categoryIds.Contains(p.CategoryId.Value))
                .ToListAsync();
            var dto = products.Select(a => new ProductDto()
            {
                ProductId = a.ProductId,
                ProductName = a.ProductName,
                Description = a.Description,
                DiscountedPrice = a.DiscountedPrice,
                DiscountPercentage = a.DiscountPercentage,
                OriginalPrice = a.OriginalPrice,
                Category = a.Category?.Name,
                ImageUrls = a.ProductImages?.Where(img => img.IsMainImage == true)
                       .Select(img => img.ImageUrl).ToList()
            }).ToList();
            return dto;
        }

    }
}
