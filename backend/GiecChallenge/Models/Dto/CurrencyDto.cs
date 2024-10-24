using System.ComponentModel.DataAnnotations;

public class CurrencyDto
{
    public Guid id { get; set; } = new Guid();

    public string ISOCode { get; set; } = string.Empty;

    public List<CurrencyNamesDto> names { get; set; } = new List<CurrencyNamesDto>();

    public string language { get; set; } = "FR";
 }

public class CurrencyNamesDto
{
    [Required]
    public string name { get; set; } = string.Empty;

    [Required]
    public string language { get; set; } = string.Empty;
}