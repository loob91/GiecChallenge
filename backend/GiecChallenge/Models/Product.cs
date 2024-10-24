using System.ComponentModel.DataAnnotations;

namespace GiecChallenge.Models;

public class Product
{
    public Guid id { get; set; }

    [Required]
    public ProductSubGroup subgroup { get; set; } = new ProductSubGroup();

    [Required]
    public List<ProductLanguage> names { get; set; } = new List<ProductLanguage>();

    [Required]
    public double CO2 { get; set; }

    [Required]
    public string CO2Unit { get; set; } = string.Empty;

    public double water { get; set; }

    public string waterUnit { get; set; } = string.Empty;

    [Required]
    public int amortization { get; set; } = 0;
 }