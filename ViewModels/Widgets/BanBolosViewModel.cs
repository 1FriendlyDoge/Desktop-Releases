using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ERM_Desktop.Models;
using ERM_Desktop.Services;
using Newtonsoft.Json;

namespace ERM_Desktop.ViewModels.Widgets;

public partial class BanBolosViewModel : WidgetViewModel
{
    public IBrush BackgroundBrush => Storage.MainVM?.BackgroundBrush ?? new SolidColorBrush(Color.FromRgb(0, 0, 0)); 
    public WindowTransparencyLevel TransparencyLevel => Storage.MainVM?.TransparencyLevel ?? WindowTransparencyLevel.None;
    
    [ObservableProperty]
    private ObservableCollection<DisplayPunishment> _punishmentsList = new ObservableCollection<DisplayPunishment>();

    [ObservableProperty]
    private int _cursor;

    [ObservableProperty]
    private bool _deletion;

    [ObservableProperty]
    private bool _loaded;
    
    [ObservableProperty]
    private bool _noItems = true;
    
    [ObservableProperty]
    private string _cursorFormatted = "0/0";
    
    public void Step(int increment)
    {
        if(increment + Cursor >= PunishmentsList.Count)
        {
            Cursor = 0;
        }
        else if(increment + Cursor < 0)
        {
            Cursor = PunishmentsList.Count - 1;
        }
        else
        {
            Cursor += increment;
        }
        
        UpdateCursorFormat();
        PunishmentsList.ElementAtOrDefault(Cursor)?.DispatchLoader();
    }

    [RelayCommand]
    public void GoNext()
    {
        Step(1);
    }
    
    [RelayCommand]
    public void GoPrevious()
    {
        Step(-1);
    }
    
    public void UpdateCursorFormat()
    {
        CursorFormatted = $"{Cursor + 1}/{PunishmentsList.Count}";
    }
    
    [RelayCommand]
    public async void CompleteBolo()
    {
        DisplayPunishment? punishment = PunishmentsList.ElementAtOrDefault(Cursor);
        
        if(punishment == null)
        {
            return;
        }
        
        punishment.Deletion = true;
        
        await Storage.HttpClient.GetAsync($"api/Moderation/CompleteBolo/{punishment.Punishment.Id}");
        
        punishment.Deletion = false;
        
        NoItems = PunishmentsList.Count == 0;
    }
    
    [RelayCommand]
    public async void CopyBan()
    {
        string? text = PunishmentsList.ElementAtOrDefault(Cursor)?.BanCommand;
        if(text == null)
        {
            return;
        }
        
        await Application.Current?.Clipboard?.SetTextAsync(text)!;
    }
    
    public void BoloAttach(Punishment p)
    {
        PunishmentsList.Insert(0, new DisplayPunishment(p));
        Dispatcher.UIThread.Post(() =>
        {
            if(Cursor == 1)
            {
                Cursor = 0;
            }
            
            UpdateCursorFormat();
        });
        
        NoItems = PunishmentsList.Count == 0;
    }
    
    public void BoloDelete(string objectId)
    {
        DisplayPunishment? dp = PunishmentsList.FirstOrDefault(x => x.Punishment.Id == objectId);
        if(dp == null)
        {
            return;
        }
        
        PunishmentsList.Remove(dp);
        UpdateCursorFormat();
        NoItems = PunishmentsList.Count == 0;
    }
    
    public override async Task OnLoadedAsync() 
    {
        HttpResponseMessage resp = await Storage.HttpClient.GetAsync($"api/Moderation/GetBoloPunishments/{Storage.DiscordServer!.ID}");
        if(!resp.IsSuccessStatusCode)
        {
            return;
        }
        
        List<Punishment>? punishments = JsonConvert.DeserializeObject<List<Punishment>>(await resp.Content.ReadAsStringAsync());
        if(punishments == null)
        {
            return;
        }

        punishments.Reverse();
        
        foreach(Punishment punishment in punishments)
        {
            PunishmentsList.Add(new DisplayPunishment(punishment, false));
        }
        
        PunishmentsList.ElementAtOrDefault(Cursor)?.DispatchLoader();
        UpdateCursorFormat();
        Loaded = true;
        NoItems = PunishmentsList.Count == 0;
    }
}