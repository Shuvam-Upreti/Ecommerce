using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mover.Areas.Admin.ViewModel.Order;
using Mover.Core.Dto.Carts;
using Mover.Core.Exceptions;
using Mover.Core.Services.Interfaces;
using Mover.Extension;
using Mover.HttpUtility;
using Mover.Logging;
using Mover.ViewModel.Carts;

namespace Mover.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUserId = SessionInfo.GetCurrentUser().Id;
                var carts = await _cartService.GetAllCarts(currentUserId);
                ViewBag.TotalPrice = carts?.FirstOrDefault()?.TotalPrice ?? 0;

                var vm = carts?.Select(a => new CartViewModel
                {
                    CartId = a.CartId,
                    ProductName = a.ProductName,
                    ProductPrice = a.ProductPrice,
                    ImageUrls = a.ImageUrl,
                    Quantity = a.Quantity

                }).ToList();
                return View(vm);
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return RedirectToAction(nameof(Index));
            }

        }
        public async Task<IActionResult> IncrementItemCount(int id)
        {
            try
            {
                await _cartService.IncreaseCount(id);
                return RedirectToAction(nameof(Index));
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyInfo(ex.Message);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return RedirectToAction(nameof(Index));
            }

        }
        public async Task<IActionResult> DecrementItemCount(int id)
        {
            try
            {
                await _cartService.DecreaseCount(id);
                return RedirectToAction(nameof(Index));
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyInfo(ex.Message);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return RedirectToAction(nameof(Index));
            }

        }
        public async Task<IActionResult> Summary()
        {
            try
            {
                var currentUser = SessionInfo.GetCurrentUser();
                var summary = await _cartService.GetSummary(currentUser);

                var vm = new SummaryViewModel
                {
                    ShippingDetails = summary.ShippingDetails == null ? null : new ShippingViewModel
                    {
                        CreaterName = summary.ShippingDetails.CreaterName,
                        ShippingAddressLine = summary.ShippingDetails.ShippingAddressLine,
                        ShippingCity = summary.ShippingDetails.ShippingCity,
                        ShippingState = summary.ShippingDetails.ShippingState,
                        ShippingZipCode = summary.ShippingDetails.ShippingZipCode
                    },
                    CartViewModel = summary.CartDto.Select(a => new CartViewModel
                    {
                        CartId = a.CartId,
                        ProductId = a.ProductId,
                        ProductName = a.ProductName,
                        Quantity = a.Quantity,
                        ProductPrice = a.ProductPrice,
                        TotalPrice = a.TotalPrice,
                    }).ToList(),
                    SummaryTotalPrice=summary.CartDto.Sum(a => a.TotalPrice)
                };


                return View(vm);
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyInfo(ex.Message);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return RedirectToAction(nameof(Index));
            }

        }
    }
}
