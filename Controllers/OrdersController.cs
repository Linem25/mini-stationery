using Microsoft.AspNetCore.Mvc;
using MiniStationery.Mvc.Services;
using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Controllers;

public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IStationeryService _stationeryService;

    public OrdersController(IOrderService orderService, IStationeryService stationeryService)
    {
        _orderService = orderService;
        _stationeryService = stationeryService;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var stationeries = await _stationeryService.GetStationeryListAsync();
        ViewBag.Stationeries = stationeries;

        var model = new OrderCreateViewModel { Quantity = 1 };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderCreateViewModel model)
    {
        try
        {
            await _orderService.CreateOrderAsync(model);
            TempData["SuccessMessage"] = "Đã tạo đơn hàng thành công.";
            return RedirectToAction(nameof(Create));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            var stationeries = await _stationeryService.GetStationeryListAsync();
            ViewBag.Stationeries = stationeries;
            return View(model);
        }
    }
}