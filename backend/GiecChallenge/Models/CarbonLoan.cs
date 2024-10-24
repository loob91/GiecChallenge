using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class CarbonLoan
{
    public Guid id { get; set; }
    
    [Required]
    public User user { get; set; } = new User();
    
    [Required]
    public ProductPurchase productPurchase { get; set; } = new ProductPurchase();
    
    [Required]
    public DateTime dateBegin { get; set; }
    
    [Required]
    public DateTime dateEnd { get; set; }
 }