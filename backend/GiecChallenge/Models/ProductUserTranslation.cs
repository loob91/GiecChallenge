using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class ProductUserTranslation
{
    public Guid id { get; set; }
    
    [Required]
    public User user { get; set; } = new User();
    
    [Required]
    public Product product { get; set; } = new Product();
    
    [Required]
    public string name { get; set; } = string.Empty;
 }