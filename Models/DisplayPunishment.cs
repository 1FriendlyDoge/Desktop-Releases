using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ERM_Desktop.Services;

namespace ERM_Desktop.Models;

public partial class DisplayPunishment : ObservableObject
{
    [ObservableProperty]
    private Punishment _punishment = null!;
    
    [ObservableProperty]
    private string _avatarUrl = String.Empty;
    
    [ObservableProperty]
    private bool _deletion;

    public string ModeratorDiscord => $"@{Punishment.Moderator}";
    public string ModeratorTooltip => $"{ModeratorDiscord} ({Punishment.ModeratorID})";
    public string DateFormatted => DateTimeOffset.FromUnixTimeSeconds(Punishment.Epoch).ToString("G") + " UTC";
    public string BanCommand => $":ban {Punishment.UserID}";

    public DisplayPunishment(Punishment punishment, bool preload = true)
    {
        Punishment = punishment;
        if(preload)
        {
            DispatchLoader();
        }
    }
    
    public void DispatchLoader()
    {
        if(AvatarUrl == String.Empty)
        {
            Task.Run(async () =>
            {
                AvatarUrl = await Roblox.ResolveAvatar(Punishment.UserID.ToString());
            });
        }
    }
    
    public DisplayPunishment(Punishment punishment, string avatarUrl)
    {
        Punishment = punishment;
        AvatarUrl = avatarUrl;
    }
}