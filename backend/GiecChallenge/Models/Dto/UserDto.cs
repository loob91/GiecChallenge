using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class UserDto
{
    [Required]
    public string email { get; set; } = string.Empty;
    
    [Required]
    public string password { get; set; } = string.Empty;
    
    public string language { get; set; } = string.Empty;


    public string token { get; set; } = string.Empty;
 }