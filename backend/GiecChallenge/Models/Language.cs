using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class Language
{
    public Guid id { get; set; }

    [Required]
    public string ISOCode { get; set; } = string.Empty;
    
    [Required]
    [InverseProperty("languageToChange")]
    public virtual List<LanguageLanguage> names { get; set; } = new List<LanguageLanguage>();
    
    [Required]
    [InverseProperty("language")]
    public virtual List<LanguageLanguage> usedToTranlateLanguage { get; set; } = new List<LanguageLanguage>();
 }