using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ERM_Desktop.Models;
using ERM_Desktop.Services;
using ERM_Desktop.ViewModels.Widgets;
using Microsoft.AspNetCore.SignalR.Client;
using ERM_Desktop.Views.Widgets;
using Newtonsoft.Json;

namespace ERM_Desktop.ViewModels;

public partial class HomeViewModel : PageViewModel
{
    public ModerationWidgetView? moderationWidgetView;
    public ModerationViewModel? moderationViewModel;
    
    public RecentLogsWidgetView? recentLogsWidgetView;
    public RecentLogsViewModel? recentLogsViewModel;
    
    public ShiftWidgetView? shiftWidgetView;
    public ShiftViewModel? shiftViewModel;

    public ActiveStaffWidgetView? activeStaffWidgetView;
    public ActiveStaffViewModel? activeStaffViewModel;

    public BanBolosWidgetView? bolosWidgetView;
    public BanBolosViewModel? bolosViewModel;
    
    public SearchWidgetView? searchWidgetView;
    public SearchViewModel? searchViewModel;
    
    public PixelPoint? moderationPosition;
    public PixelPoint? recentLogsPosition;
    public PixelPoint? shiftPosition;
    public PixelPoint? activeStaffPosition;
    public PixelPoint? bolosPosition;
    public PixelPoint? searchPosition;

    [ObservableProperty]
    private bool _profilesLoaded;

    [ObservableProperty]
    private bool _profileCreating;
    
    [ObservableProperty]
    private bool _profileOperation;

    private string DiscordGuildID = String.Empty;
    
    // Global View Bindings

    [ObservableProperty]
    private bool _modEnabled;
    
    [ObservableProperty]
    private bool _modPinned;
    
    [ObservableProperty]
    private bool _modLocked;

    partial void OnModEnabledChanged(bool value)
    {
        if(value)
        {
            moderationViewModel = new ModerationViewModel();
            moderationWidgetView = new ModerationWidgetView(this)
            {
                DataContext = moderationViewModel
            };

            if (moderationPosition != null)
            {
                moderationWidgetView.Position = moderationPosition.Value;
            }
            
            moderationWidgetView.Show();
            moderationViewModel.OnLoadedAsync();
        }
        else
        {
            moderationPosition = moderationWidgetView?.Position;
            moderationWidgetView?.Close();
            moderationWidgetView = null;
        }
    }


    [ObservableProperty]
    private bool _searchEnabled;
    
    [ObservableProperty]
    private bool _searchPinned;
    
    [ObservableProperty]
    private bool _searchLocked;

    async partial void OnSearchEnabledChanged(bool value)
    {
        if(value)
        {
            searchViewModel = new SearchViewModel();
            searchWidgetView = new SearchWidgetView(this)
            {
                DataContext = searchViewModel
            };
            
            if(searchPosition != null)
            {
                searchWidgetView.Position = searchPosition.Value;
            }
            
            searchWidgetView.Show();
            await searchViewModel.OnLoadedAsync();
        }
        else
        {
            searchPosition = searchWidgetView?.Position;
            searchWidgetView?.Close();
            searchWidgetView = null;
        }
    }


    [ObservableProperty]
    private bool _shiftEnabled;
    
    [ObservableProperty]
    private bool _shiftPinned;
    
    [ObservableProperty]
    private bool _shiftLocked;

    async partial void OnShiftEnabledChanged(bool value)
    {
        if(value)
        {
            shiftViewModel = new ShiftViewModel();
            shiftWidgetView = new ShiftWidgetView(this)
            {
                DataContext = shiftViewModel
            };
            
            if(shiftPosition != null)
            {
                shiftWidgetView.Position = shiftPosition.Value;
            }
            
            shiftWidgetView.Show();
            await shiftViewModel.OnLoadedAsync();
        }
        else
        {
            shiftPosition = shiftWidgetView?.Position;
            shiftWidgetView?.Close();
            shiftWidgetView = null;
        }
    }


    [ObservableProperty]
    private bool _activeEnabled;
    
    [ObservableProperty]
    private bool _activePinned;
    
    [ObservableProperty]
    private bool _activeLocked;

    async partial void OnActiveEnabledChanged(bool value)
    {
        if(value)
        {
            activeStaffViewModel = new ActiveStaffViewModel();
            activeStaffWidgetView = new ActiveStaffWidgetView(this)
            {
                DataContext = activeStaffViewModel
            };
            
            if(activeStaffPosition != null)
            {
                activeStaffWidgetView.Position = activeStaffPosition.Value;
            }
            
            activeStaffWidgetView.Show();
            await activeStaffViewModel.OnLoadedAsync();
        }
        else
        {
            activeStaffPosition = activeStaffWidgetView?.Position;
            activeStaffWidgetView?.Close();
            activeStaffWidgetView = null;
        }
    }


    [ObservableProperty]
    private bool _recentEnabled;
    
    [ObservableProperty]
    private bool _recentPinned;
    
    [ObservableProperty]
    private bool _recentLocked;

    async partial void OnRecentEnabledChanged(bool value)
    {
        if(value)
        {
            recentLogsViewModel = new RecentLogsViewModel();
            recentLogsWidgetView = new RecentLogsWidgetView(this)
            {
                DataContext = recentLogsViewModel
            };
            
            if(recentLogsPosition != null)
            {
                recentLogsWidgetView.Position = recentLogsPosition.Value;
            }
            
            recentLogsWidgetView.Show();
            await recentLogsViewModel.OnLoadedAsync();
        }
        else
        {
            recentLogsPosition = recentLogsWidgetView?.Position;
            recentLogsWidgetView?.Close();
            recentLogsWidgetView = null;
        }
    }
    
    
    [ObservableProperty]
    private bool _bolosEnabled;
    
    [ObservableProperty]
    private bool _bolosPinned;
    
    [ObservableProperty]
    private bool _bolosLocked;

    async partial void OnBolosEnabledChanged(bool value)
    {
        if(value)
        {
            bolosViewModel = new BanBolosViewModel();
            bolosWidgetView = new BanBolosWidgetView(this)
            {
                DataContext = bolosViewModel
            };
            
            if(bolosPosition != null)
            {
                bolosWidgetView.Position = bolosPosition.Value;
            }
            
            bolosWidgetView.Show();
            await bolosViewModel.OnLoadedAsync();
        }
        else
        {
            bolosPosition = bolosWidgetView?.Position;
            bolosWidgetView?.Close();
            bolosWidgetView = null;
        }
    }


    public IBrush BackgroundBrush => Storage.MainVM?.BackgroundBrush ?? new SolidColorBrush(Color.FromRgb(0, 0, 0)); 
    public WindowTransparencyLevel TransparencyLevel => Storage.MainVM?.TransparencyLevel ?? WindowTransparencyLevel.None;
    
    
    // Home View Bindings
    
    public string AvatarURL => $"{Storage.UserInstance?.AvatarURL}?size=80";
    public string Greeting => $"Hey, {Storage.UserInstance?.Username}!";

    public string ServerIcon => Storage.DiscordServer != null
        ? Storage.DiscordServer.IconURL
        : "https://cdn.discordapp.com/embed/avatars/4.png";

    public string ServerName => Storage.DiscordServer?.Name ?? "Invalid Server";

    [ObservableProperty]
    private string _profileInput = String.Empty;

    [ObservableProperty]
    private ObservableCollection<Profile> _profiles = new ObservableCollection<Profile>();
    
    [ObservableProperty]
    private bool _wsConnected;
    
    public Profile ExportProfile()
    {
        Profile p = new Profile
        {
            ModerationState = new Profile.WidgetState
            {
                Enabled = ModEnabled,
                Pinned = ModPinned,
                Locked = ModLocked
            },
            RecentLogsState = new Profile.WidgetState
            {
                Enabled = RecentEnabled,
                Pinned = RecentPinned,
                Locked = RecentLocked
            },
            ShiftState = new Profile.WidgetState
            {
                Enabled = ShiftEnabled,
                Pinned = ShiftPinned,
                Locked = ShiftLocked
            },
            ActiveStaffState = new Profile.WidgetState
            {
                Enabled = ActiveEnabled,
                Pinned = ActivePinned,
                Locked = ActiveLocked
            },
            BolosState = new Profile.WidgetState
            {
                Enabled = BolosEnabled,
                Pinned = BolosPinned,
                Locked = BolosLocked
            },
            SearchState = new Profile.WidgetState
            {
                Enabled = SearchEnabled,
                Pinned = SearchPinned,
                Locked = SearchLocked
            },
            ModerationPosition = moderationWidgetView?.Position ?? moderationPosition ?? new PixelPoint(0, 0),
            RecentLogsPosition = recentLogsWidgetView?.Position ?? recentLogsPosition ?? new PixelPoint(0, 0),
            ShiftPosition = shiftWidgetView?.Position ?? shiftPosition ?? new PixelPoint(0, 0),
            ActiveStaffPosition = activeStaffWidgetView?.Position ?? activeStaffPosition ?? new PixelPoint(0, 0),
            BolosPosition = bolosWidgetView?.Position ?? bolosPosition ?? new PixelPoint(0, 0),
            SearchPosition = searchWidgetView?.Position ?? searchPosition ?? new PixelPoint(0, 0)
        };

        return p;
    }
    
    [RelayCommand]
    public void ApplyProfile(Profile p)
    {
        ProfileOperation = true;
        
        moderationPosition = p.ModerationPosition;
        recentLogsPosition = p.RecentLogsPosition;
        shiftPosition = p.ShiftPosition;
        activeStaffPosition = p.ActiveStaffPosition;
        bolosPosition = p.BolosPosition;
        searchPosition = p.SearchPosition;
        
        if(moderationWidgetView != null)
        {
            moderationWidgetView.Position = p.ModerationPosition;
        }
        if(recentLogsWidgetView != null)
        {
            recentLogsWidgetView.Position = p.RecentLogsPosition;
        }
        if(shiftWidgetView != null)
        {
            shiftWidgetView.Position = p.ShiftPosition;
        }
        if(activeStaffWidgetView != null)
        {
            activeStaffWidgetView.Position = p.ActiveStaffPosition;
        }
        if(bolosWidgetView != null)
        {
            bolosWidgetView.Position = p.BolosPosition;
        }
        if(searchWidgetView != null)
        {
            searchWidgetView.Position = p.SearchPosition;
        }

        ModEnabled = p.ModerationState.Enabled;
        ModPinned = p.ModerationState.Pinned;
        ModLocked = p.ModerationState.Locked;
        
        RecentEnabled = p.RecentLogsState.Enabled;
        RecentPinned = p.RecentLogsState.Pinned;
        RecentLocked = p.RecentLogsState.Locked;
        
        ShiftEnabled = p.ShiftState.Enabled;
        ShiftPinned = p.ShiftState.Pinned;
        ShiftLocked = p.ShiftState.Locked;
        
        ActiveEnabled = p.ActiveStaffState.Enabled;
        ActivePinned = p.ActiveStaffState.Pinned;
        ActiveLocked = p.ActiveStaffState.Locked;
        
        BolosEnabled = p.BolosState.Enabled;
        BolosPinned = p.BolosState.Pinned;
        BolosLocked = p.BolosState.Locked;
        
        SearchEnabled = p.SearchState.Enabled;
        SearchPinned = p.SearchState.Pinned;
        SearchLocked = p.SearchState.Locked;
        
        ProfileOperation = false;
    }

    [RelayCommand]
    public async void AddProfile()
    {
        ProfileOperation = true;
        
        ProfileCreating = true;
        if(String.IsNullOrWhiteSpace(ProfileInput))
        {
            ProfileCreating = false;
            ProfileOperation = false;
            return;
        }

        Profile p = ExportProfile();
        p.Name = ProfileInput;

        HttpResponseMessage resp = await Storage.HttpClient.PostAsync("api/Profile/CreateProfile",
            new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json"));
        
        if(!resp.IsSuccessStatusCode)
        {
            ProfileCreating = false;
            ProfileOperation = false;
            return;
        }
        
        p.Id = await resp.Content.ReadAsStringAsync();
        
        Profiles.Insert(0, p);
        ProfileInput = String.Empty;
        
        ProfileCreating = false;
        ProfileOperation = false;
    }

    [RelayCommand]
    public async void RemoveProfile(Profile profile)
    {
        ProfileOperation = true;
        
        HttpResponseMessage resp = await Storage.HttpClient.DeleteAsync($"api/Profile/DeleteProfile/{profile.Id}");
        
        if(!resp.IsSuccessStatusCode)
        {
            ProfileOperation = false;
            return;
        }

        Profiles.Remove(profile);
        ProfileOperation = false;
    }
    
    [RelayCommand]
    public async void UpdateProfile(Profile profile)
    {
        ProfileOperation = true;
        
        Profile p = ExportProfile();
        p.Name = profile.Name;
        p.Id = profile.Id;
        
        HttpResponseMessage resp = await Storage.HttpClient.PutAsync($"api/Profile/UpdateProfile/{profile.Id}",
            new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json"));
        
        if(!resp.IsSuccessStatusCode)
        {
            ProfileOperation = false;
            return;
        }
        
        int index = Profiles.IndexOf(profile);
        Profiles.Remove(profile);
        Profiles.Insert(index, p);
        
        ProfileOperation = false;
    }

    [RelayCommand]
    public static void SwitchAccount()
    {
        Utility.SwitchAccount();
    }

    [RelayCommand]
    public static void SwitchServer()
    {
        Utility.SwitchServer();
    }
    
    public void BridgeSearch(long robloxId, string username)
    {
        if(searchWidgetView is not { IsVisible: true })
        {
            SearchEnabled = true;
        }
        
        if(searchWidgetView != null)
        {
            searchWidgetView.Topmost = true;
            searchWidgetView.Topmost = SearchPinned;
            searchWidgetView?.Focus();
        }
        
        searchViewModel?.FetchSearch(long.Parse(DiscordGuildID), robloxId, username);
    }
    
    public void HideSearch()
    {
        SearchEnabled = false;
    }
    

    public override async Task OnNavigatedToAsync()
    {
        DiscordGuildID = Storage.DiscordServer!.ID;

        await Task.Run(async () =>
        {
            HttpResponseMessage resp = await Storage.HttpClient.GetAsync("api/Profile/GetProfiles");

            if (!resp.IsSuccessStatusCode)
            {
                return;
            }

            List<Profile> profiles =
                JsonConvert.DeserializeObject<List<Profile>>(await resp.Content.ReadAsStringAsync()) ??
                new List<Profile>();

            foreach (Profile p in profiles)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    Profiles.Insert(0, p);
                });
            }
            
            ProfilesLoaded = true;
        }).ConfigureAwait(false);
        
        await Task.Run(() =>
        {
            try
            {
                Storage.DiscordRpcClient.UpdateState("Moderating");
                Storage.DiscordRpcClient.UpdateDetails(Storage.DiscordServer.Name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }).ConfigureAwait(false);
        
        if(Storage.HubConnection.State == HubConnectionState.Disconnected)
        {
            await Storage.HubConnection.StartAsync();
        }
        
        Storage.HubConnection.Closed += (_) =>
        {
            Storage.MainVM?.NavigateToPageByViewModelAsync(new HomeViewModel()).Wait();
            return Task.CompletedTask;
        };
        
        Storage.HubConnection.Reconnecting += (_) =>
        {
            WsConnected = false;
            return Task.CompletedTask;
        };

        await Storage.HubConnection.SendAsync("Register", Storage.Identifier, DiscordGuildID);
        
        Storage.HubConnection.Reconnected += async (_) =>
        {
            await Storage.HubConnection.SendAsync("Register", Storage.Identifier, DiscordGuildID);
            WsConnected = true;
        };
        
        Storage.HubConnection.On("Punishment", (Punishment punishment) =>
        {
            recentLogsViewModel?.AddPunishment(new DisplayPunishment(punishment));
            if(punishment.Type?.ToLower() == "bolo")
            {
                bolosViewModel?.BoloAttach(punishment);
            }
            searchViewModel?.WsInsertPunishment(punishment);
        });
        
        Storage.HubConnection.On("PunishmentDeletion", (string objectId, Punishment? punishment) =>
        {
            recentLogsViewModel?.SwapDelete(objectId, punishment);
            bolosViewModel?.BoloDelete(objectId);
            searchViewModel?.DeletePunishment(objectId);
        });
        
        Storage.HubConnection.On("ShiftStarted" , (Shift shift) =>
        {
            if(shift.UserID.ToString() == Storage.UserInstance?.DiscordID && shift.Guild.ToString() == Storage.DiscordServer.ID)
            {
                shiftViewModel?.StartShift(shift);
            }
            
            activeStaffViewModel?.AddStartedShift(shift);
        });
        
        Storage.HubConnection.On("ShiftEnded" , (long userid, long guild) =>
        {
            if(userid.ToString() == Storage.UserInstance?.DiscordID && guild.ToString() == Storage.DiscordServer.ID)
            {
                shiftViewModel?.EndShift();
            }
            
            activeStaffViewModel?.RemoveShift(userid, guild);
        });

        Storage.HubConnection.On("BreakStarted", (Shift shift) =>
        {
            if(shift.UserID.ToString() == Storage.UserInstance?.DiscordID && shift.Guild.ToString() == Storage.DiscordServer.ID)
            {
                shiftViewModel?.StartBreak(shift);
            }
            
            activeStaffViewModel?.ModifyBreak(shift);
        });
        
        Storage.HubConnection.On("BreakEnded", (Shift shift) =>
        {
            if(shift.UserID.ToString() == Storage.UserInstance?.DiscordID && shift.Guild.ToString() == Storage.DiscordServer.ID)
            {
                shiftViewModel?.EndBreak(shift);
            }
            
            activeStaffViewModel?.ModifyBreak(shift);
        });
        
        WsConnected = true;
    }
    
    public override async Task OnNavigatedFromAsync()
    {
        if(Storage.HubConnection.State == HubConnectionState.Connected)
        {
            await Storage.HubConnection.SendAsync("Unregister", Storage.Identifier, DiscordGuildID);
        }
        
        moderationWidgetView?.Close();
        recentLogsWidgetView?.Close();
        shiftWidgetView?.Close();
        activeStaffWidgetView?.Close();
        bolosWidgetView?.Close();
        searchWidgetView?.Close();
    }
}