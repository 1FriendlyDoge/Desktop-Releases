using System;
using System.Net.Http;
using DiscordRPC;
using ERM_Desktop.Models;
using ERM_Desktop.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;

namespace ERM_Desktop.Services;

public static class Storage
{
    public static readonly string BaseUrl = "https://api.ermbot.xyz/";

    public static bool OAuth2Loaded = false;
    public static string Identifier = String.Empty;

    public static UserInstance? UserInstance = null;
    
    public static DiscordServer? DiscordServer = null;

    public static MainWindowViewModel? MainVM = null;
    
    public static HomeViewModel? HomeVM = null;
    
    public static bool ExperimentalMode = false;

    public static bool PreviousAuth = false;

    public static string? BanReason = null;
    
    public static readonly HttpClient HttpClient = new HttpClient() { BaseAddress = new Uri(BaseUrl) };
    
    public static readonly HubConnection HubConnection = new HubConnectionBuilder()
        .WithUrl(Storage.BaseUrl + "socket")
        .WithAutomaticReconnect(new[] { TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(100),TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(400), TimeSpan.FromMilliseconds(500)})
        .Build();
    
    public static readonly DiscordRpcClient DiscordRpcClient = new DiscordRpcClient("1090291300881932378");
    public static DateTime InitialLoadTime = DateTime.UtcNow;
}