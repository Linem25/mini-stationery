using MiniStationery.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiniStationery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StationeryController : ControllerBase
{
    private readonly StationeryService _stationeryService;

    public StationeryController(StationeryService stationeryService)
    {
        _stationeryService = stationeryService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var items = _stationeryService.GetAll()
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Category,
                s.Brand,
                s.Price,
                s.Quantity,
                Status = _stationeryService.GetStockStatus(s.Quantity)
            });
        return Ok(items);
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(_stationeryService.GetStats());
    }
}