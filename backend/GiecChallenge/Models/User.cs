using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class User
{
    public Guid id { get; set; }

    [Required]
    public string email { get; set; } = string.Empty;
    
    [Required]
    public string password { get; set; } = string.Empty;

    public Language favoriteLanguage { get; set; } = new Language();

    public DateTime creationDate { get; set; }

    public byte[] hash { get; set; } = new byte[] {};

    public List<UserInGroup> groups { get; set; } = new List<UserInGroup>();
 }