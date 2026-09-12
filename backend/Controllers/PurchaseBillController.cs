using backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("api/purchase-bill")]
public class PurchaseBillController : ControllerBase
{
    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] PurchaseBillCalculateRequestDto request)
    {
        // Standard Cost × Quantity
        var baseCost = request.StandardCost * request.Quantity;
        
        // Discount percentage applied to that cost amount
        var discountAmount = baseCost * (request.DiscountPercentage / 100m);
        
        var totalCost = baseCost - discountAmount;
        
        // Standard Price × Quantity
        var totalSelling = request.StandardPrice * request.Quantity;

        var response = new PurchaseBillCalculateResponseDto
        {
            Item = request.Item,
            LocationCode = request.LocationCode,
            StandardCost = request.StandardCost,
            StandardPrice = request.StandardPrice,
            Quantity = request.Quantity,
            DiscountPercentage = request.DiscountPercentage,
            TotalCost = totalCost,
            TotalSelling = totalSelling
        };

        return Ok(response);
    }
}
