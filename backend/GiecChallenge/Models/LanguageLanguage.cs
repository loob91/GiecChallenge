using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class LanguageLanguage
{
    public Guid id { get; set; }

    [Required]
    [ForeignKey("names")]
    public Language languageToChange { get; set; } = new Language();

    [Required]
    [ForeignKey("usedToTranlateLanguage")]
    public Language language { get; set; } = new Language();

    [Required]
    public string name { get; set; } = string.Empty;
 }