using System;
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
using DiscordRPC;
using ERM_Desktop.Models;
using ERM_Desktop.Services;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace ERM_Desktop.ViewModels.Widgets;

public partial class ShiftViewModel : WidgetViewModel
{
    public IBrush BackgroundBrush => Storage.MainVM?.BackgroundBrush ?? new SolidColorBrush(Color.FromRgb(0, 0, 0)); 
    public WindowTransparencyLevel TransparencyLevel => Storage.MainVM?.TransparencyLevel ?? WindowTransparencyLevel.None;
    
    [ObservableProperty]
    private bool _onShift;
    
    [ObservableProperty]
    private DateTime _shiftStart = DateTime.UtcNow;
    
    [ObservableProperty]
    private string _formattedTime = "Loading";

    [ObservableProperty]
    private bool _loaded;

    [ObservableProperty]
    private ObservableCollection<string> _shiftTypes = new() { "Default" };

    [ObservableProperty]
    private int _selectedType;

    [ObservableProperty]
    private string _status = "None";
    
    [ObservableProperty]
    private Shift? _shift;
    
    [ObservableProperty]
    private string _shiftText = "Start Shift";
    
    [ObservableProperty]
    private string _breakText = "Start Break";

    [ObservableProperty]
    private IBrush? _ellipseColor = new SolidColorBrush(Color.FromRgb(235, 69, 71));

    private bool OnBreak;
    
    private readonly SolidColorBrush _offShiftBrush = new SolidColorBrush(Color.FromRgb(235, 69, 71));
    private readonly SolidColorBrush _onShiftBrush = new SolidColorBrush(Color.FromRgb(78, 179, 229));
    private readonly SolidColorBrush _breakBrush = new SolidColorBrush(Color.FromRgb(255,140,0));

    private readonly Timer _timer = new Timer(1000);


    public override async Task OnLoadedAsync()
    {
        Task<HttpResponseMessage> respTask = Storage.HttpClient.GetAsync($"api/Shift/GetShiftTypes/{Storage.DiscordServer?.ID}");
        Task<HttpResponseMessage> shiftTask = Storage.HttpClient.GetAsync($"api/Shift/GetActiveShift/{Storage.DiscordServer?.ID}");
        
        HttpResponseMessage resp = await respTask;
        
        if(resp.IsSuccessStatusCode)
        {
            List<string> tempList = JsonConvert.DeserializeObject<List<string>>(await resp.Content.ReadAsStringAsync()) ?? new();
        
            if(tempList.Count > 0)
            {
                ShiftTypes = new ObservableCollection<string>(tempList);
            }
        }

        SelectedType = 0;
        
        _timer.Elapsed += TimerOnElapsed;
        
        HttpResponseMessage shiftResp = await shiftTask;

        if (shiftResp.IsSuccessStatusCode)
        {
            Shift = JsonConvert.DeserializeObject<Shift>(await shiftResp.Content.ReadAsStringAsync());

            if (Shift == null)
            {
                return;
            }

            int idx = ShiftTypes.IndexOf(Shift.Type!);

            if (idx != -1)
            {
                SelectedType = idx;
            }

            OnShift = true;
            
            Dispatcher.UIThread.Post(() =>
            {
                EllipseColor = _onShiftBrush;
            });

            if(Shift.Breaks.Any(x => x.EndEpoch == 0))
            {
                await Task.Run(() =>
                {
                    try
                    {
                        double? startEpoch = Shift.Breaks.FirstOrDefault(x => x.EndEpoch == 0)?.StartEpoch;
                        
                        Storage.DiscordRpcClient.UpdateState("On-Break");
                        Storage.DiscordRpcClient.UpdateStartTime(startEpoch == null ? Storage.InitialLoadTime : DateTimeOffset.FromUnixTimeSeconds((long)startEpoch).UtcDateTime);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }).ConfigureAwait(false);
                
                FormattedTime = CalulateTime();
                Status = "Break";
                BreakText = "End Break";
                OnBreak = true;
                
                Dispatcher.UIThread.Post(() =>
                {
                    EllipseColor = _breakBrush;
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    try
                    {
                        Storage.DiscordRpcClient.UpdateState("On-Shift");
                        Storage.DiscordRpcClient.UpdateStartTime(DateTimeOffset.FromUnixTimeSeconds((long)Shift.StartEpoch).UtcDateTime);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }).ConfigureAwait(false);
                
                _timer.Start();
                Status = "On-Shift";
                BreakText = "Start Break";
            }
            
            ShiftText = "End Shift";
        }
        else
        {
            OnShift = false;
            FormattedTime = "00:00:00";
            Status = "Off-Shift";
            ShiftText = "Start Shift";
        }

        Loaded = true;
    }
    
    [RelayCommand]
    public async void InteractShift()
    {
        Loaded = false;
        
        if(OnShift)
        {
            await Storage.HttpClient.GetAsync($"api/Shift/EndShift/{Storage.DiscordServer?.ID}");
        }
        else
        {
            Shift shift = new()
            {
                Type = ShiftTypes[SelectedType],
                StartEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Guild = long.Parse(Storage.DiscordServer!.ID),
                UserID = long.Parse(Storage.UserInstance!.DiscordID),
                Username = Storage.UserInstance.Username,
                Nickname = Storage.UserInstance.Username
            };
            
            StringContent content = new StringContent(JsonConvert.SerializeObject(shift), System.Text.Encoding.UTF8, "application/json");

            await Storage.HttpClient.PostAsync($"api/Shift/StartShift", content);
        }
        
        Loaded = true;
    }
    
    [RelayCommand]
    public async void InteractBreak()
    {
        Loaded = false;
        
        if(OnBreak)
        {
            await Storage.HttpClient.GetAsync($"api/Shift/EndBreak/{Storage.DiscordServer?.ID}");
        }
        else
        {
            await Storage.HttpClient.GetAsync($"api/Shift/StartBreak/{Storage.DiscordServer?.ID}");
        }
        
        Loaded = true;
    }
    
    public void StartShift(Shift shift)
    {
        Shift = shift;
        OnShift = true;
        OnBreak = false;
        ShiftText = "End Shift";
        Status = "On-Shift";
        
        Task.Run(() =>
        {
            try
            {
                Storage.DiscordRpcClient.UpdateState("On-Shift");
                Storage.DiscordRpcClient.UpdateStartTime(DateTimeOffset.FromUnixTimeSeconds((long)shift.StartEpoch).UtcDateTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }).ConfigureAwait(false);

        int idx = ShiftTypes.IndexOf(shift.Type ?? string.Empty);
        SelectedType = idx != -1 ? idx : 0;
        
        _timer.Start();
        
        Dispatcher.UIThread.Post(() =>
        {
            EllipseColor = _onShiftBrush;
        });
    }
    
    public void EndShift()
    {
        OnShift = false;
        OnBreak = false;
        ShiftText = "Start Shift";
        Status = "Off-Shift";
        _timer.Stop();
        FormattedTime = "00:00:00";
        
        Task.Run(() =>
        {
            try
            {
                Storage.DiscordRpcClient.UpdateState("Moderating");
                Storage.DiscordRpcClient.UpdateStartTime(Storage.InitialLoadTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }).ConfigureAwait(false);
        
        Dispatcher.UIThread.Post(() =>
        {
            EllipseColor = _offShiftBrush;
        });
    }
    
    public void StartBreak(Shift shift)
    {
        Shift = shift;
        OnBreak = true;
        Status = "Break";
        BreakText = "End Break";
        _timer.Stop();
        FormattedTime = CalulateTime();
        
        Task.Run(() =>
        {
            try
            {
                double? startEpoch = shift.Breaks.FirstOrDefault(x => x.EndEpoch == 0)?.StartEpoch;
                DateTime start = startEpoch == null ? Storage.InitialLoadTime : DateTimeOffset.FromUnixTimeSeconds((long)startEpoch).UtcDateTime;
                
                Storage.DiscordRpcClient.UpdateState("On-Break");
                Storage.DiscordRpcClient.UpdateStartTime(start);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }).ConfigureAwait(false);
        
        Dispatcher.UIThread.Post(() =>
        {
            EllipseColor = _breakBrush;
        });
    }
    
    public void EndBreak(Shift shift)
    {
        Shift = shift;
        OnShift = true;
        OnBreak = false;
        Status = "On-Shift";
        BreakText = "Start Break";
        _timer.Start();
        
        Task.Run(() =>
        {
            try
            {
                double totalBreakTime = 0;
        
                foreach(Break b in Shift.Breaks)
                {
                    DateTime endEpoch = b.EndEpoch == 0 ? DateTime.UtcNow : DateTimeOffset.FromUnixTimeSeconds((long)b.EndEpoch).UtcDateTime;
                    
                    totalBreakTime += (long)(endEpoch - DateTimeOffset.FromUnixTimeSeconds((long)b.StartEpoch).UtcDateTime).TotalSeconds;
                }

                TimeSpan timeSpan = (DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds((long)Shift.StartEpoch)).Subtract(TimeSpan.FromSeconds(totalBreakTime));
                
                DateTime start = DateTime.UtcNow - timeSpan;
                
                Storage.DiscordRpcClient.UpdateState("On-Shift");
                Storage.DiscordRpcClient.UpdateStartTime(start);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }).ConfigureAwait(false);
        
        Dispatcher.UIThread.Post(() =>
        {
            EllipseColor = _onShiftBrush;
        });
    }
    
    public string CalulateTime()
    {
        if(Shift == null)
        {
            return "00:00:00";
        }
        
        double totalBreakTime = 0;
        
        foreach(Break b in Shift.Breaks)
        {
            DateTime endEpoch = b.EndEpoch == 0 ? DateTime.UtcNow : DateTimeOffset.FromUnixTimeSeconds((long)b.EndEpoch).UtcDateTime;
                    
            totalBreakTime += (long)(endEpoch - DateTimeOffset.FromUnixTimeSeconds((long)b.StartEpoch).UtcDateTime).TotalSeconds;
        }

        TimeSpan timeSpan = (DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds((long)Shift.StartEpoch)).Subtract(TimeSpan.FromSeconds(totalBreakTime));
        
        if(timeSpan.TotalMilliseconds < 0)
        {
            return "00:00:00";
        }
        
        return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        if(OnShift && Shift != null)
        {
            Dispatcher.UIThread.Post(() =>
            {
                FormattedTime = CalulateTime();
            });
        }
    }
}