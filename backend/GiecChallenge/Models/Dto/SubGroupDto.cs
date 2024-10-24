using System.ComponentModel.DataAnnotations;

public class SubGroupDto
{
    public Guid id { get; set; } = new Guid();

    public List<SubGroupNamesDto> names { get; set; } = new List<SubGroupNamesDto>();

    [Required]
    public string group { get; set; } = string.Empty;

    public string language { get; set; } = "FR";
 }

public class SubGroupNamesDto
{
    [Required]
    public string name { get; set; } = string.Empty;

    [Required]
    public string language { get; set; } = string.Empty;
}