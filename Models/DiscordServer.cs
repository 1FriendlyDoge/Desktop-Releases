using System;

namespace ERM_Desktop.Models;

public class DiscordServer
{
    // ReSharper disable once MemberInitializerValueIgnored
    public string IconURL { get; set; } = String.Empty;
    public string Name { get; set; }
    public string ID { get; set; }

    public int MemberCount { get; set; }
    public int PermissionLevel { get; set; }
    public string InfoFormatted => $"Members: {MemberCount}, Permission: {(PermissionLevel == 2 ? "Management" : PermissionLevel == 1 ? "Moderator" : "Member")}";
    
    
    public DiscordServer(string iconUrl, string name, string id, int memberCount, int permissionLevel = 0)
    {
        IconURL = iconUrl;
        Name = name;
        ID = id;
        MemberCount = memberCount;
        PermissionLevel = permissionLevel;
    }
}