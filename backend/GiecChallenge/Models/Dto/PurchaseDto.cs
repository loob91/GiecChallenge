using System.ComponentModel.DataAnnotations;

namespace GiecChallenge.Models;

public class PurchaseDto
{
    public Guid? id { get; set; } = Guid.Empty;

    [Required]
    public DateTime datePurchase { get; set; }

    [Required]
    public List<ProductPurchaseDto> products { get; set; } = new List<ProductPurchaseDto>();

    public double CO2Cost { get; set; } = 0;

    public double WaterCost { get; set; } = 0;
 }

public class PurchaseLaRucheDto
{
    public DateTime datePurchase { get; set; }

    public string command { get; set; } = string.Empty;
 }