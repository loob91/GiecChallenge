using System.ComponentModel.DataAnnotations;

public class LanguageDto
{
    public Guid id { get; set; } = new Guid();

    public string ISOCode { get; set; } = string.Empty;

    public List<LanguageNamesDto> names { get; set; } = new List<LanguageNamesDto>();

    public string language { get; set; } = "FR";
 }

public class LanguageNamesDto
{
    [Required]
    public string name { get; set; } = string.Empty;

    [Required]
    public string language { get; set; } = string.Empty;
}