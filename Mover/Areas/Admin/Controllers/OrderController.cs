using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mover.Areas.Admin.ViewModel.Order;
using Mover.Areas.Admin.ViewModel.Product;
using Mover.Controllers;
using Mover.Core.Dto.Category;
using Mover.Core.Dto.Filter;
using Mover.Core.Dto.Order;
using Mover.Core.Exceptions;
using Mover.Core.Services.Implementations;
using Mover.Core.Services.Interfaces;
using Mover.Extension;
using Mover.HttpUtility;
using Mover.Logging;
using Mover.ViewModel.Carts;
using Mover.ViewModel.Filter;
using static Mover.Core.Enums.Orders.OrderStatus;

namespace Mover.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly GetGuestIdOrSessionUser _userHelper;
        public OrderController(IOrderService orderService, GetGuestIdOrSessionUser userHelper, IProductService productService)
        {
            _orderService = orderService;
            _userHelper=userHelper;
            _productService=productService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var (guestId, currentUser) = await _userHelper.GetGuestIdOrSessionUserId();
                var orderstatus = Enum.GetValues<OrderStatusEnums>().Cast<OrderStatusEnums>()
                                .Select(a => new SelectListItem
                                {
                                    Value = a.ToString(),
                                    Text = a.ToString(),
                                }).ToList();
                ViewBag.OrderStatus = orderstatus;
                var orders = await _orderService.GetAllOrders(currentUser, guestId);
                var vm = orders.Select(a => new OrderViewModel()
                {
                    OrderId = a.OrderId,
                    CreatedBy = a.CreatedBy,
                    PhoneNumber = a.PhoneNumber,
                    TotalAmount = a.TotalAmount,
                    OrderDate = a.OrderDate,
                    OrderStatus = a.OrderStatus,
                    OrderItemsViewModel= a.OrderItemsDto.Select(a => new OrderItemViewModel()
                    {
                        ProductName=a.ProductName,
                        Quantity=a.Quantity,
                        PriceAtPurchase=a.PriceAtPurchase,
                        ImageUrl=a.ImageUrl
                    }).ToList()
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
        public async Task<IActionResult> LoadOrders(FilterViewModel model, string? orderStatus, string? filterDateFrom, string? filterDateTo, string? searchInput)
        {
            try
            {
                var dto = new FilterDto()
                {
                    Search = model.Search,
                    PageSize = model.PageSize,
                    PageIndex = model.PageIndex
                };
                var (guestId, currentUser) = await _userHelper.GetGuestIdOrSessionUserId();
                var (orderList, totalCount) = await _orderService.GetAllOrdersForGrid(dto, currentUser, orderStatus, guestId, filterDateFrom, filterDateTo, searchInput);
                var datas = orderList.Select(a => new OrderViewModel
                {
                    OrderId = a.OrderId,
                    CreatedBy = a.CreatedBy,
                    PhoneNumber = a.PhoneNumber,
                    TotalAmount = a.TotalAmount,
                    OrderDate = a.OrderDate,
                    OrderStatus = a.OrderStatus,
                    ShippingAddressLine=a.ShippingAddressLine,
                    CurrentUserRole = currentUser.Role

                }).ToList();
                var result = Json(new { data = datas, totalCount = totalCount });
                return result;

            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong.Please try again");
                return View();
            }
        }

        public async Task<IActionResult> CreateOrder()
        {
            var products = await _productService.GetAllProducts(); 

            var summaryVm = new SummaryViewModel
            {
                CartViewModel = products.Select(p => new CartViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductPrice = p.DiscountedPrice,
                    DiscountPercentage = p.DiscountPercentage,
                    Quantity = 0 
                }).ToList()
            };

            return PartialView("~/Areas/Admin/Views/Order/Partial/_CreateOrder.cshtml", summaryVm);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(SummaryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.NotifyModelStateErrors();
                return RedirectToAction("Summary", "Cart", new { area = "" });
            }

            if (model.ShippingDetails.ShippingAddressLine==null||model.ShippingDetails.PhoneNumber==null)
            {
                this.NotifyInfo("Address and phone number cannot be null.");
                return RedirectToAction("Summary", "Cart", new { area = "" });
            }
            try
            {

                var (guestId, currentUser) = await _userHelper.GetGuestIdOrSessionUserId();

                var dto = new OrderDto()
                {
                    UserId = currentUser?.Id,
                    GuestId=guestId,
                    PhoneNumber=model.ShippingDetails.PhoneNumber,
                    GuestEmail=model.ShippingDetails.GuestEmail,
                    OrderDate = DateTime.Now,
                    ShippingAddressLine = model.ShippingDetails.ShippingAddressLine,
                    ShippingCity = model.ShippingDetails.ShippingCity,
                    ShippingZipCode = model.ShippingDetails.ShippingZipCode,
                    TotalAmount = model.SummaryTotalPrice,
                    ShippingState = model.ShippingDetails.ShippingState,
                    CreatedBy=model.ShippingDetails.CreaterName,
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

                return PartialView("~/Areas/Admin/Views/Order/Partial/_EditOrderStatus.cshtml", vm);

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

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var order = await _orderService.GetOrderById(id);

                var vm = new OrderViewModel()
                {
                    OrderId = order.OrderId,
                    CreatedBy = order.CreatedBy,
                    PhoneNumber = order.PhoneNumber,
                    TotalAmount = order.TotalAmount,
                    OrderDate = order.OrderDate,
                    OrderStatus = order.OrderStatus,
                    ShippingAddressLine = order.ShippingAddressLine,
                    ShippingCity = order.ShippingCity,
                    ShippingZipCode = order.ShippingZipCode,
                    PaymentStatus = order.PaymentStatus,
                    ShippingState = order.ShippingState,
                    OrderItemsViewModel = order.OrderItemsDto.Select(a => new OrderItemViewModel()
                    {
                        DiscountAtPurchase = a.DiscountAtPurchase,
                        PriceAtPurchase = a.PriceAtPurchase,
                        Quantity = a.Quantity,
                        OrderItemId = a.OrderItemId,
                        ProductId = a.ProductId,
                        ProductName=a.ProductName,
                        ImageUrl=a.ImageUrl
                    }).ToList()
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
                this.NotifyError("Something went wrong.Please try again");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _orderService.Delete(id);
                this.NotifyError("Order canceled suessfully.");
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
