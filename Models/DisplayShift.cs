using System;
using System.Linq;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using ERM_Desktop.Services;

namespace ERM_Desktop.Models;

public sealed partial class DisplayShift : ObservableObject
{
    [ObservableProperty]
    private Shift _shift = null!;
    
    [ObservableProperty]
    private IBrush _indicatorColor = new SolidColorBrush(Color.FromRgb(235, 69, 71));

    [ObservableProperty]
    private bool _onBreak;

    [ObservableProperty]
    private string _formattedTime = "00:00:00";
    
    [ObservableProperty]
    private string _formattedTotalTime = "00:00:00";
    
    [ObservableProperty]
    private string _formattedBreakTime = "00:00:00";
    
    [ObservableProperty]
    private string _breakText = "00:00:00";

    [ObservableProperty]
    private bool _executing;
    public bool OwnedShift => Shift.UserID.ToString() == Storage.UserInstance?.DiscordID;

    private static readonly SolidColorBrush ShiftBrush = new SolidColorBrush(Color.FromRgb(78, 179, 229));
    private static readonly SolidColorBrush BreakBrush = new SolidColorBrush(Color.FromRgb(255,140,0));

    public DisplayShift(Shift shift)
    {
        Shift = shift;
    }
    
    public void UpdateTime()
    {
        double totalBreakTime = 0;
        
        foreach(Break b in Shift.Breaks)
        {
            DateTime endEpoch = b.EndEpoch == 0 ? DateTime.UtcNow : DateTimeOffset.FromUnixTimeSeconds((long)b.EndEpoch).UtcDateTime;
                    
            totalBreakTime += (long)(endEpoch - DateTimeOffset.FromUnixTimeSeconds((long)b.StartEpoch).UtcDateTime).TotalSeconds;
        }

        TimeSpan timeSpan = (DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds((long)Shift.StartEpoch)).Subtract(TimeSpan.FromSeconds(totalBreakTime));
        
        if(timeSpan.TotalMilliseconds < 0)
        {
            FormattedTime = "00:00:00";
        }

        FormattedTime = $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}{(timeSpan.TotalHours > 24 ? "+" : "")}{(timeSpan.TotalHours > 24 ? (long)Math.Floor(timeSpan.TotalDays) : "")}{(timeSpan.TotalHours > 24 ? "D" : "")}";
    }
    
    public void UpdateState()
    {
        if(Shift.EndEpoch != 0)
        {
            return;
        }
        
        if(Shift.Breaks.Any(b => b.EndEpoch == 0))
        {
            OnBreak = true;
            IndicatorColor = BreakBrush;
            BreakText = "End Break";
        }
        else
        {
            OnBreak = false;
            IndicatorColor = ShiftBrush;
            BreakText = "Start Break";
        }
    }
    
    public void UpdateMenuTimes()
    {
        TimeSpan tt = DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds((long)Shift.StartEpoch).UtcDateTime;
        
        double totalBreakTime = 0;
        
        foreach(Break b in Shift.Breaks)
        {
            DateTime endEpoch = b.EndEpoch == 0 ? DateTime.UtcNow : DateTimeOffset.FromUnixTimeSeconds((long)b.EndEpoch).UtcDateTime;
                    
            totalBreakTime += (long)(endEpoch - DateTimeOffset.FromUnixTimeSeconds((long)b.StartEpoch).UtcDateTime).TotalSeconds;
        }
        TimeSpan bt = TimeSpan.FromSeconds(totalBreakTime);
        
        FormattedTotalTime = $"Total Time: {tt.Hours:00}:{tt.Minutes:00}:{tt.Seconds:00}{(tt.TotalHours > 24 ? "+" : "")}{(tt.TotalHours > 24 ? (long)Math.Floor(tt.TotalDays) : "")}{(tt.TotalHours > 24 ? "D" : "")}";
        FormattedBreakTime = $"Break Time: {bt.Hours:00}:{bt.Minutes:00}:{bt.Seconds:00}{(bt.TotalHours > 24 ? "+" : "")}{(bt.TotalHours > 24 ? (long)Math.Floor(bt.TotalDays) : "")}{(bt.TotalHours > 24 ? "D" : "")}";
    }
}