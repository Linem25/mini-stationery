using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Services;

public interface IOrderService
{
    Task CreateOrderAsync(OrderCreateViewModel model);
}