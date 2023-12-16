using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
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

public partial class ModerationViewModel : WidgetViewModel
{
    [ObservableProperty]
    public IBrush _backgroundBrush = Storage.MainVM?.BackgroundBrush ?? new SolidColorBrush(Color.FromRgb(0, 0, 0)); 
    
    [ObservableProperty]
    public WindowTransparencyLevel _transparencyLevel = Storage.MainVM?.TransparencyLevel ?? WindowTransparencyLevel.None;

    [ObservableProperty]
    private string _usernameInput = String.Empty;
    
    [ObservableProperty]
    private string _reasonInput = String.Empty;

    [ObservableProperty] private int _actionIndex;
    
    [ObservableProperty]
    private string _avatarUrl = String.Empty;
    
    [ObservableProperty]
    private string _usernameResult = "None";
    
    [ObservableProperty]
    private string _userIdResult = "None";
    
    [ObservableProperty]
    private string _aiPrediction = String.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _autocompleteResults = new ObservableCollection<string>();

    [ObservableProperty]
    private bool _completesOpen;
    
    [ObservableProperty]
    private bool _focus;

    [ObservableProperty]
    private bool _searchEnabled;
    
    [ObservableProperty]
    private object? _selectedItem;

    [ObservableProperty]
    private List<string> _punishmentTypes = new List<string>()
    {
        "Warning",
        "Kick",
        "Ban",
        "Bolo"
    };

    [ObservableProperty]
    private bool _idlePunishing = true;

    public override Task OnLoadedAsync()
    {
        // Load punishment types from API
        
        usernameTimer.Start();
        aiTimer.Start();
        idleTimer.Start();
        
        usernameTimer.Elapsed += async (_, _) =>
        {
            await UsernameTimerElapsed();
        };
        
        aiTimer.Elapsed += async (_, _) =>
        {
            await AiTimerElapsed();
        };
        
        idleTimer.Elapsed += async (_, _) =>
        {
            await IdleTimerElapsed();
        };

        return Task.CompletedTask;
    }
    
    private readonly Timer usernameTimer = new Timer(100);
    private readonly Timer aiTimer = new Timer(100);
    private readonly Timer idleTimer = new Timer(500);
    private string usernameCheckpoint = String.Empty;
    private string aiCheckpoint = String.Empty;
    private long completionCheckpoint;

    // goofy ah timer has no reset function lol
    partial void OnUsernameInputChanged(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            SelectedItem = null;
            AutocompleteResults.Clear();
        }

        
        if(AutocompleteResults.Contains(value))
        {
            Task.Run(async () =>
            {
                await ForceUserlookup();
            });
        }

        if(SelectedItem != null)
        {
            usernameTimer.Stop();
        }
        else
        {
            usernameTimer.Stop();
            usernameTimer.Start();
        }
        
        idleTimer.Stop();
        idleTimer.Start();
    }
    
    public async Task ForceUserlookup()
    {
        if(UsernameInput == UsernameResult || UsernameInput.Length < 3 || string.IsNullOrWhiteSpace(UsernameInput))
        {
            return;
        }
        
        UserSearch? userSearch = await Roblox.GetUser(UsernameInput);
            
        if(userSearch == null)
        {
            return;
        }
            
        Dispatcher.UIThread.Post(() =>
        {
            UsernameResult = userSearch.name;
            UserIdResult = userSearch.id.ToString();
            AvatarUrl = String.Empty;
            SearchEnabled = true;
        });

        string avatar = await Roblox.ResolveAvatar(userSearch.id.ToString(), 140);
            
        Dispatcher.UIThread.Post(() =>
        {
            AvatarUrl = avatar;
        });
    }
    
    partial void OnReasonInputChanged(string value)
    {
        aiTimer.Stop();
        aiTimer.Start();
        
        if(value == String.Empty)
        {
            AiPrediction = String.Empty;
        }
    }
    
    private async Task UsernameTimerElapsed()
    {

        if(usernameCheckpoint == UsernameInput)
        {
            return;
        }
        
        usernameCheckpoint = UsernameInput;
        long startEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        
        if(UsernameInput.Length < 3 || string.IsNullOrWhiteSpace(UsernameInput))
        {
            return;
        }
        
        List<string>? results = await Roblox.AutocompleteUsername(UsernameInput);

        Dispatcher.UIThread.Post(() =>
        {
            if(results is { Count: > 0 } && startEpoch > completionCheckpoint)
            {
                completionCheckpoint = startEpoch;
                AutocompleteResults = new ObservableCollection<string>(results);
                
                if(Focus)
                {
                    CompletesOpen = true;
                }
            }
        });
    }
    
    private async Task AiTimerElapsed()
    {
        if(aiCheckpoint == ReasonInput)
        {
            return;
        }
        
        aiCheckpoint = ReasonInput;
        
        if(ReasonInput.Length == 0)
        {
            return;
        }
        
        try
        {
            AiPrediction = await Roblox.PredictPunishment(ReasonInput);
        }
        catch
        {
            AiPrediction = "Error";
        }
    }
    
    private async Task IdleTimerElapsed()
    {
        await ForceUserlookup();
    }
    
    [RelayCommand]
    public void SearchUser()
    {
        if(UserIdResult == "None")
        {
            return;
        }
        
        Dispatcher.UIThread.Post(() =>
        {
            Storage.HomeVM?.BridgeSearch(long.Parse(UserIdResult), UsernameResult);
        });
    }

    [RelayCommand]
    public void ClearInputs()
    {
        ReasonInput = String.Empty;
        ActionIndex = 0;
        AiPrediction = String.Empty;
    }
    
    [RelayCommand]
    public async void PunishAI()
    {
        if(!IdlePunishing && AiPrediction == String.Empty)
        {
            return;
        }

        IdlePunishing = false;
        
        Punishment punishment = new Punishment()
        {
            Username = UsernameInput,
            Type = AiPrediction,
            Reason = ReasonInput,
            Moderator = Storage.UserInstance!.Username,
            ModeratorID = long.Parse(Storage.UserInstance.DiscordID),
            Guild = long.Parse(Storage.DiscordServer!.ID)
        };
        
        StringContent postContent = new StringContent(JsonConvert.SerializeObject(punishment), Encoding.UTF8, "application/json");
        
        HttpResponseMessage resp = await Storage.HttpClient.PostAsync("api/Moderation/CreatePunishment", postContent);
        
        if(resp.IsSuccessStatusCode && UsernameInput == punishment.Username)
        {
            ClearInputs();
        }

        IdlePunishing = true;
    }
    
    [RelayCommand]
    public async void Punish()
    {
        if(!IdlePunishing)
        {
            return;
        }

        IdlePunishing = false;
        
        Punishment punishment = new Punishment()
        {
            Username = UsernameInput,
            Type = PunishmentTypes.ElementAt(ActionIndex),
            Reason = ReasonInput,
            Moderator = Storage.UserInstance!.Username,
            ModeratorID = long.Parse(Storage.UserInstance.DiscordID),
            Guild = long.Parse(Storage.DiscordServer!.ID)
        };
        
        StringContent postContent = new StringContent(JsonConvert.SerializeObject(punishment), Encoding.UTF8, "application/json");
        
        HttpResponseMessage resp = await Storage.HttpClient.PostAsync("api/Moderation/CreatePunishment", postContent);
        
        if(resp.IsSuccessStatusCode && UsernameInput == punishment.Username)
        {
            ClearInputs();
        }

        IdlePunishing = true;
    }
}