using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class Currency
{
    public Guid id { get; set; }
    
    [Required]
    public string ISOCode { get; set; } = string.Empty;
    
    [Required]
    public List<CurrencyLanguage> names { get; set; } = new List<CurrencyLanguage>();
 }