using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ERM_Desktop.Models;
using ERM_Desktop.Services;
using Newtonsoft.Json;

namespace ERM_Desktop.ViewModels.Widgets;

public partial class ActiveStaffViewModel : WidgetViewModel
{
    public IBrush BackgroundBrush => Storage.MainVM?.BackgroundBrush ?? new SolidColorBrush(Color.FromRgb(0, 0, 0)); 
    public WindowTransparencyLevel TransparencyLevel => Storage.MainVM?.TransparencyLevel ?? WindowTransparencyLevel.None;
    public bool ManagementPermissions => Storage.DiscordServer?.PermissionLevel == 2;
    
    [ObservableProperty]
    private ObservableCollection<DisplayShift> _shifts = new();
    
    [ObservableProperty]
    private bool _loaded;

    [ObservableProperty]
    private bool _inactive;

    private readonly Timer _timer = new Timer(1000);

    public override async Task OnLoadedAsync()
    {
        HttpResponseMessage resp = await Storage.HttpClient.GetAsync($"api/Shift/GetActiveShifts/{Storage.DiscordServer!.ID}");
        
        if(resp.IsSuccessStatusCode)
        {
            List<Shift>? shifts = JsonConvert.DeserializeObject<List<Shift>>(await resp.Content.ReadAsStringAsync());
            
            if(shifts != null)
            {
                foreach(Shift s in shifts)
                {
                    DisplayShift ds = new DisplayShift(s);
                    ds.UpdateState();
                    ds.UpdateTime();
                    ds.UpdateMenuTimes();
                    
                    Shifts.Add(ds);
                }
            }
        }

        Shifts = new ObservableCollection<DisplayShift>(Shifts.OrderByDescending(x => x.Shift.StartEpoch));

        Loaded = true;
        
        Inactive = Shifts.Count == 0;
        
        _timer.Elapsed += TimerOnElapsed;
        _timer.Start();
    }
    
    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        foreach(DisplayShift s in Shifts)
        {
            s.UpdateTime();
            s.UpdateMenuTimes();
        }
    }
    
    [RelayCommand]
    public async void ForceEndShift(DisplayShift shift)
    {
        shift.Executing = true;
        
        await Storage.HttpClient.GetAsync($"api/Shift/ForceEndShift/{Storage.DiscordServer?.ID}/{shift.Shift.UserID}");
        
        shift.Executing = false;
    }
    
    [RelayCommand]
    public async void ForceModifyBreak(DisplayShift shift)
    {
        shift.Executing = true;
        
        if(shift.OnBreak)
        {
            await Storage.HttpClient.GetAsync($"api/Shift/ForceEndBreak/{Storage.DiscordServer?.ID}/{shift.Shift.UserID}");
        }
        else
        {
            await Storage.HttpClient.GetAsync($"api/Shift/ForceStartBreak/{Storage.DiscordServer?.ID}/{shift.Shift.UserID}");
        }

        shift.Executing = false;
    }
    
    [RelayCommand]
    public async void ForceVoidShift(DisplayShift shift)
    {
        shift.Executing = true;
        
        await Storage.HttpClient.GetAsync($"api/Shift/ForceVoidShift/{Storage.DiscordServer?.ID}/{shift.Shift.UserID}");
        
        shift.Executing = false;
    }
    
    public void AddStartedShift(Shift shift)
    {
        Dispatcher.UIThread.Post(() =>
        {
            DisplayShift ds = new DisplayShift(shift);
            ds.UpdateState();
            ds.UpdateTime();
            ds.UpdateMenuTimes();

            Shifts.Insert(0, ds);
            Shifts = new ObservableCollection<DisplayShift>(Shifts.OrderByDescending(x => x.Shift.StartEpoch));
            
            Inactive = Shifts.Count == 0;
        });
    }
    
    public void RemoveShift(long userid, long guild)
    {
        DisplayShift? shift = Shifts.FirstOrDefault(x => x.Shift.UserID == userid && x.Shift.Guild == guild);
        if(shift != null)
        {
            Shifts.Remove(shift);
        }
        
        Inactive = Shifts.Count == 0;
    }
    
    public void ModifyBreak(Shift shift)
    {
        DisplayShift? ds = Shifts.FirstOrDefault(x => x.Shift.UserID == shift.UserID && x.Shift.Guild == shift.Guild);
        
        if(ds != null)
        {
            ds.Shift = shift;
            ds.UpdateState();
        }
    }
}