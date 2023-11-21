using System.Collections.Generic;

namespace ERM_Desktop.Models;

public class UsernameResolvePost
{
    public List<string> usernames { get; set; } = new List<string>();
    public bool excludeBannedUsers { get; set; }
}