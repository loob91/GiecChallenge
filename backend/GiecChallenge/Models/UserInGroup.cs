namespace GiecChallenge.Models;

public class UserInGroup
{
    public Guid id { get; set; }

    public User user { get; set; } = new User();

    public UserGroup userGroup { get; set; } = new UserGroup();
 }