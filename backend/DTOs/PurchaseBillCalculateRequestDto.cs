using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class PurchaseBillCalculateRequestDto
{
    [Required]
    [AllowedValues("Mango", "Apple", "Banana", "Orange", "Grapes", "Kiwi", "Strawberry", ErrorMessage = "Invalid item selection.")]
    public string Item { get; set; } = string.Empty;

    [Required]
    public string LocationCode { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Standard Cost must be greater than 0.")]
    public decimal StandardCost { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Standard Price must be greater than 0.")]
    public decimal StandardPrice { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }

    [Range(0, 100, ErrorMessage = "Discount Percentage must be between 0 and 100.")]
    public decimal DiscountPercentage { get; set; }
}
