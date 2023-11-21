using System;
using System.Collections.Generic;

namespace ERM_Desktop.Models;

public class UserInstance
{
    public string Id { get; set; } = String.Empty;
    public string Token { get; set; } = String.Empty;
    public string Username { get; set; } = String.Empty;
    public string Discriminator { get; set; } = String.Empty;
    public string DiscordID { get; set; } = String.Empty;
    public string AvatarURL { get; set; } = String.Empty;

    public List<DiscordServer>? DiscordServers { get; set; } = new List<DiscordServer>();
}