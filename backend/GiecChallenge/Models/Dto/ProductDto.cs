using System.ComponentModel.DataAnnotations;

public class ProductDto
{
    public Guid id {get; set;} = new Guid();

    public List<ProductNamesDto> names { get; set; } = new List<ProductNamesDto>();

    public string language { get; set; } = "FR";

    public string group { get; set; } = string.Empty;

    public double CO2 { get; set; }

    public string CO2Unit { get; set; } = string.Empty;

    public double water { get; set; }

    public string waterUnit { get; set; } = string.Empty;

    public int amortization { get; set; } = 0;
 }

public class ProductNamesDto
{
    [Required]
    public string name { get; set; } = string.Empty;

    [Required]
    public string language { get; set; } = string.Empty;
}

public class ProductUserTranslationDTO
{
    public string id { get; set; } = string.Empty;
    
    [Required]
    public string user { get; set; } = string.Empty;
    
    [Required]
    public string product { get; set; } = string.Empty;
    
    [Required]
    public string name { get; set; } = string.Empty;
}