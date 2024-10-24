using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class ProductPurchase
{
    public Guid id { get; set; }
    
    [Required]
    public Purchase purchase { get; set; } = new Purchase();

    [Required]
    public Product product { get; set; } = new Product();

    [Required]
    public Currency currency { get; set; } = new Currency();

    [Required]
    public Double price { get; set; }

    [Required]
    public Double quantity { get; set; }

    [Required]
    public Double CO2Cost { get; set; }
 }