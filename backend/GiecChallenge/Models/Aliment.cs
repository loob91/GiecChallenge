using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class Aliment : Product
{
    [Required]
    public string ciqual { get; set; } = default!;
 }