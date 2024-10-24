using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class UserGroupDto
{
    public Guid id { get; set; }

    public string name { get; set; } = string.Empty;
 }