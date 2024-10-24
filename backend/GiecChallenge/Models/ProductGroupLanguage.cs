using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class ProductGroupLanguage
{
    public Guid id { get; set; }
    
    [Required]
    public ProductGroup productgroup { get; set; } = new ProductGroup();
    
    [Required]
    public Language language { get; set; } = new Language();
    
    [Required]
    public string name { get; set; } = string.Empty;
 }