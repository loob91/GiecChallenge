using System.ComponentModel.DataAnnotations;

public class GroupDto
{
    public Guid id { get; set; } = new Guid();

    public List<GroupNamesDto> names { get; set; } = new List<GroupNamesDto>();

    public string language { get; set; } = "FR";
 }

public class GroupNamesDto
{
    [Required]
    public string name { get; set; } = string.Empty;

    [Required]
    public string language { get; set; } = string.Empty;
}