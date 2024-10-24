using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class ProductPurchaseDto
{
    public string id { get; set; } = string.Empty;

    public string translation { get; set; } = string.Empty!;

    public string product { get; set; } = string.Empty!;

    public string currencyIsoCode { get; set; } = string.Empty!;

    public Guid? productId { get; set; }

    public Double quantity { get; set; }

    public Double price { get; set; }

    public Double WaterCost { get; set; }

    public Double CO2Cost { get; set; }
 }

public class PurchaseLaRucheImportReturnDto
{
    public Guid id { get; set; }

    public List<ProductPurchaseDto> productsToTranslate { get; set; } = new List<ProductPurchaseDto>();
}