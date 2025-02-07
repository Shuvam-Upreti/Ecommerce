using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mover.Areas.Admin.ViewModel.Order;
using Mover.Areas.Admin.ViewModel.Product;
using Mover.Controllers;
using Mover.Core.Dto.Category;
using Mover.Core.Dto.Order;
using Mover.Core.Exceptions;
using Mover.Core.Services.Implementations;
using Mover.Core.Services.Interfaces;
using Mover.Extension;
using Mover.HttpUtility;
using Mover.Logging;
using Mover.ViewModel.Carts;
using static Mover.Core.Enums.Orders.OrderStatus;

namespace Mover.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = SessionInfo.GetCurrentUser();
                var orders = await _orderService.GetAllOrders(currentUser);
                var vm = orders.Select(a => new OrderViewModel()
                {
                    OrderId = a.OrderId,
                    CreatedBy = a.CreatedBy,
                    PhoneNumber = a.PhoneNumber,
                    TotalAmount = a.TotalAmount,
                    OrderDate = a.OrderDate,
                    OrderStatus = a.OrderStatus,
                }).ToList();
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

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(SummaryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.NotifyModelStateErrors();
                return RedirectToAction("Summary", "Cart", new { area = "" });
            }
            try
            {

                var currentUser = SessionInfo.GetCurrentUser();
                var dto = new OrderDto()
                {
                    UserId = currentUser.Id,
                    OrderDate = DateTime.Now,
                    ShippingAddressLine = model.ShippingDetails.ShippingAddressLine,
                    ShippingCity = model.ShippingDetails.ShippingCity,
                    ShippingZipCode = model.ShippingDetails.ShippingZipCode,
                    TotalAmount = model.SummaryTotalPrice,
                    ShippingState = model.ShippingDetails.ShippingState,
                    OrderItemsDto = model.CartViewModel.Select(a => new OrderItemDto
                    {
                        ProductId = a.ProductId,
                        Quantity = a.Quantity,
                        PriceAtPurchase = a.ProductPrice,
                        DiscountAtPurchase = a.DiscountPercentage
                    }).ToList()
                };


                await _orderService.Save(dto);

                this.NotifySuccess("Sucessfully placed order");
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
        public async Task<IActionResult> EditOrderStatus(int id)
        {
            try
            {
                var orderStatus = Enum.GetValues<OrderStatusEnums>().Cast<OrderStatusEnums>()
                                .Select(a => new SelectListItem
                                {
                                    Value = a.ToString(),
                                    Text = a.ToString(),
                                }).ToList();

                ViewBag.OrderStatus = orderStatus;
                var vm = new OrderStatusViewModel()
                {
                    OrderId = id
                };

                return PartialView("~/Areas/Admin/Views/Order/Partial/_EditOrderStatus.cshtml",vm);

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
                this.NotifyError("Something went wrong.Please try again");
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(OrderStatusViewModel model)
        {
            try
            {
                var dto = new OrderStatusDto()
                {
                    OrderId = model.OrderId,
                    OrderStatus = model.OrderStatus
                };
                await _orderService.UpdateOrderStatus(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong.Please try again");
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _orderService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong.Please try again");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
