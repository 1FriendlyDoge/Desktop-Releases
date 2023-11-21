using System;

namespace ERM_Desktop.Models;

public class RobloxUser
{
    public string Username { get; set; }
    public long UserID { get; set; }
    public string AvatarURL { get; set; } = String.Empty;

    public RobloxUser(string username, long userId)
    {
        Username = username;
        UserID = userId;
    }
}