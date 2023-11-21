using System.Diagnostics;
using System.Runtime.InteropServices;
using ERM_Desktop.ViewModels;

namespace ERM_Desktop.Services;

public static class Utility
{
    public static void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
    
    public static async void SwitchAccount()
    {
        Storage.OAuth2Loaded = false;
        Storage.UserInstance = null;
        Storage.DiscordServer = null;

        await Storage.MainVM?.NavigateToPageByViewModelAsync(new AuthViewModel())!;
    }
    
    public static async void SwitchServer()
    {
        Storage.DiscordServer = null;
        await Storage.MainVM?.NavigateToPageByViewModelAsync(new ServerSelectionViewModel())!;
    }
}