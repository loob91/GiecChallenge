using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class ProductGroup
{
    public Guid id { get; set; }
    
    [Required]
    public List<ProductGroupLanguage> names { get; set; } = new List<ProductGroupLanguage>();
 }