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

public partial class RecentLogsViewModel : WidgetViewModel
{
    public IBrush BackgroundBrush => Storage.MainVM?.BackgroundBrush ?? new SolidColorBrush(Color.FromRgb(0, 0, 0)); 
    public WindowTransparencyLevel TransparencyLevel => Storage.MainVM?.TransparencyLevel ?? WindowTransparencyLevel.None;
    public bool ManagementPermissions => Storage.DiscordServer?.PermissionLevel == 2;

    [ObservableProperty]
    private bool _noItems = true;

    [ObservableProperty]
    private bool _loading = true;

    public override async Task OnLoadedAsync() 
    {
        HttpResponseMessage resp = await Storage.HttpClient.GetAsync($"api/Moderation/GetLatestPunishments/{Storage.DiscordServer!.ID}");
        if(!resp.IsSuccessStatusCode)
        {
            return;
        }
        
        List<Punishment>? punishments = JsonConvert.DeserializeObject<List<Punishment>>(await resp.Content.ReadAsStringAsync());
        if(punishments == null)
        {
            return;
        }
        
        foreach(Punishment punishment in punishments)
        {
            AddPunishment(new DisplayPunishment(punishment), true, true);
        }
        
        Loading = false;
        
        if(PunishmentsList.Count > 0)
        {
            NoItems = false;
        }
    }
    
    [ObservableProperty]
    private ObservableCollection<DisplayPunishment> _punishmentsList = new ObservableCollection<DisplayPunishment>();

    public void AddPunishment(DisplayPunishment punishment, bool inititalizer = false, bool imagePreloaded = false, bool append = false)
    {
        if(!inititalizer && Loading)
        {
            return;
        }
        
        if(append)
        {
            PunishmentsList.Add(punishment);
        }
        else
        {
            PunishmentsList.Insert(0, punishment);
        }
        
        if(PunishmentsList.Count > 20)
        {
            PunishmentsList.RemoveAt(PunishmentsList.Count - 1);
        }
        
        if(!imagePreloaded)
        {
            DisplayPunishment? swapObject = PunishmentsList.FirstOrDefault(x => x.Punishment.Id == punishment.Punishment.Id);
            if(swapObject == null)
            {
                return;
            }
            
            int objIdx = PunishmentsList.IndexOf(swapObject);
            
            DisplayPunishment finalObject = new DisplayPunishment(punishment.Punishment);
            PunishmentsList[objIdx] = finalObject;
        }
        
        if(PunishmentsList.Count > 0)
        {
            NoItems = false;
        }
    }
    
    public void SwapDelete(string punishmentId, Punishment? punishment)
    {
        DisplayPunishment? deleteObject = PunishmentsList.FirstOrDefault(x => x.Punishment.Id == punishmentId);
        if(deleteObject == null)
        {
            return;
        }
        
        PunishmentsList.Remove(deleteObject);
        if(punishment != null)
        {
            AddPunishment(new DisplayPunishment(punishment), false, false, true);
        }
        
        if(PunishmentsList.Count == 0)
        {
            NoItems = true;
        }
    }
    
    [RelayCommand]
    public async void DeletePunishment(DisplayPunishment punishment)
    {
        punishment.Deletion = true;
        
        await Storage.HttpClient.DeleteAsync($"api/Moderation/DeletePunishment/{punishment.Punishment.Guild}/{punishment.Punishment.Id}");
        
        punishment.Deletion = false;
    }
}