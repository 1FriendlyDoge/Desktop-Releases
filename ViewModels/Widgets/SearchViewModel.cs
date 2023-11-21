using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ERM_Desktop.Models;
using ERM_Desktop.Services;
using Newtonsoft.Json;

namespace ERM_Desktop.ViewModels.Widgets;

public partial class SearchViewModel : WidgetViewModel
{
    public IBrush BackgroundBrush => Storage.MainVM?.BackgroundBrush ?? new SolidColorBrush(Color.FromRgb(0, 0, 0)); 
    public WindowTransparencyLevel TransparencyLevel => Storage.MainVM?.TransparencyLevel ?? WindowTransparencyLevel.None;
    public bool ManagementPermissions => Storage.DiscordServer?.PermissionLevel == 2;
    
    [ObservableProperty]
    private ObservableCollection<DisplayPunishment> _punishmentsList = new ObservableCollection<DisplayPunishment>();
    
    [ObservableProperty]
    private bool _noItems = true;

    [ObservableProperty]
    private bool _loaded = true;

    [ObservableProperty]
    private string _username = "Username: None";
    
    [ObservableProperty]
    private string _userID = "User ID: None";
    
    [ObservableProperty]
    private string _results = "Results: None";
    
    [ObservableProperty]
    private string _avatar = String.Empty;
    
    private long userId;
    
    [RelayCommand]
    public async void DeletePunishment(DisplayPunishment punishment)
    {
        punishment.Deletion = true;
        
        await Storage.HttpClient.DeleteAsync($"api/Moderation/DeletePunishment/{punishment.Punishment.Guild}/{punishment.Punishment.Id}");
        
        punishment.Deletion = false;
    }
    
    public async void FetchSearch(long guild, long robloxId, string username)
    {
        PunishmentsList.Clear();
        
        Username = $"Username: {username}";
        UserID = $"User ID: {robloxId}";
        Results = "Results: None";
        Avatar = String.Empty;
        
        userId = robloxId;
        
        Loaded = false;
        
        HttpResponseMessage resp = await Storage.HttpClient.GetAsync($"api/Moderation/SearchPunishments/{guild}/{robloxId}");
        
        if(!resp.IsSuccessStatusCode)
        {
            return;
        }
        
        List<Punishment> punishments = JsonConvert.DeserializeObject<List<Punishment>>(await resp.Content.ReadAsStringAsync()) ?? new List<Punishment>();
        
        foreach(Punishment punishment in punishments)
        {
            PunishmentsList.Insert(0, new DisplayPunishment(punishment));
        }
        
        Results = $"Results: {punishments.Count}";
        if(punishments.Count > 0)
        {
            Username = $"Username: {punishments[0].Username}";
            UserID = $"User ID: {punishments[0].UserID}";

            PunishmentsList[0].PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == "AvatarUrl")
                {
                    Avatar = PunishmentsList[0].AvatarUrl;
                }
            };
        }
        
        NoItems = PunishmentsList.Count == 0;
        Loaded = true;
    }
    
    [RelayCommand]
    public void Hide()
    {
        Storage.HomeVM?.HideSearch();
    }
    
    public void DeletePunishment(string id)
    {
        DisplayPunishment? deleteObject = PunishmentsList.FirstOrDefault(x => x.Punishment.Id == id);
        if(deleteObject == null)
        {
            return;
        }
        
        PunishmentsList.Remove(deleteObject);
        
        NoItems = PunishmentsList.Count == 0;
        Results = $"Results: {PunishmentsList.Count}";
    }
    
    public void InsertPunishment(Punishment p)
    {
        PunishmentsList.Insert(0, new DisplayPunishment(p));
        
        NoItems = PunishmentsList.Count == 0;
        Results = $"Results: {PunishmentsList.Count}";
        
        if(PunishmentsList.Count > 0)
        {
            Username = $"Username: {PunishmentsList[0].Punishment.Username}";
            UserID = $"User ID: {PunishmentsList[0].Punishment.UserID}";
            PunishmentsList[0].PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == "AvatarUrl")
                {
                    Avatar = PunishmentsList[0].AvatarUrl;
                }
            };
        }
    }
    
    public void WsInsertPunishment(Punishment p)
    {
        if(p.UserID != userId)
        {
            return;
        }
        
        InsertPunishment(p);
    }

    public override Task OnLoadedAsync()
    {
        return Task.CompletedTask;
    }
}