using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class CurrencyLanguage
{
    public Guid id { get; set; }
    
    [Required]
    public Currency currency { get; set; } = new Currency();
    
    [Required]
    public Language language { get; set; } = new Language();
    
    [Required]
    public string name { get; set; } = string.Empty;
 }