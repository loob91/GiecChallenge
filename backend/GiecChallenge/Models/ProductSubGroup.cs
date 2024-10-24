using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class ProductSubGroup
{
    public Guid id { get; set; }
    
    [Required]
    public List<ProductSubGroupLanguage> names { get; set; } = new List<ProductSubGroupLanguage>();
    
    [Required]
    public ProductGroup Groupe { get; set; } = new ProductGroup();
 }