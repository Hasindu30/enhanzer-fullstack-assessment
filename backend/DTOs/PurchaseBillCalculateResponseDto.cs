namespace backend.DTOs;

public class PurchaseBillCalculateResponseDto
{
    public string Item { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public decimal StandardCost { get; set; }
    public decimal StandardPrice { get; set; }
    public int Quantity { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalSelling { get; set; }
}
