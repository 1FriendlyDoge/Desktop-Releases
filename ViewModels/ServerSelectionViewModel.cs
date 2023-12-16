using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ERM_Desktop.Models;
using ERM_Desktop.Services;

namespace ERM_Desktop.ViewModels;

public partial class ServerSelectionViewModel : PageViewModel
{ 
    public string AvatarURL => $"{Storage.UserInstance?.AvatarURL}?size=80";
    public string Username => $"{Storage.UserInstance?.Username}";

    [ObservableProperty]
    private ObservableCollection<DiscordServer>? _servers;
    
    [ObservableProperty]
    private bool _loaded;

    public int[] SkeletonCount => new int[5];
    
    [RelayCommand]
    public async void SelectServer(DiscordServer server)
    {
        Storage.DiscordServer = server;
        
        HomeViewModel home = new HomeViewModel();
        Storage.HomeVM = home;
        await Storage.MainVM?.NavigateToPageByViewModelAsync(Storage.HomeVM)!;
    }
    
    [RelayCommand]
    public static void SwitchAccount()
    {
        Utility.SwitchAccount();
    }
    

    public override async Task OnNavigatedToAsync()
    {
        await Task.Run(() =>
        {
            try
            {
                Storage.DiscordRpcClient.UpdateState("Idle");
                Storage.DiscordRpcClient.UpdateDetails(String.Empty);
                Storage.DiscordRpcClient.UpdateStartTime(Storage.InitialLoadTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }).ConfigureAwait(false);
        
        await Auth.GetServers();
        Servers = new ObservableCollection<DiscordServer>(Storage.UserInstance?.DiscordServers ?? new List<DiscordServer>());
        Loaded = true;
    }

    public override Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}