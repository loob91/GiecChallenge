using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class ProductLanguage
{
    public Guid id { get; set; }
    
    [Required]
    public Language language { get; set; } = new Language();
    
    [Required]
    public string name { get; set; } = string.Empty;
 }