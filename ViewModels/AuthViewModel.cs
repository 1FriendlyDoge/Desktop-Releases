using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ERM_Desktop.Services;

namespace ERM_Desktop.ViewModels;

public partial class AuthViewModel : PageViewModel
{
    [ObservableProperty]
    private string _authStatus = "Awaiting OAuth2 Authorization...";
    
    [ObservableProperty]
    private string _authIdentifier = Storage.Identifier;

    public override async Task OnNavigatedToAsync()
    {
        if(!Storage.OAuth2Loaded)
        {
            Storage.OAuth2Loaded = true;
            
            if(Storage.Identifier != String.Empty && !Storage.PreviousAuth)
            {
                AuthStatus = "Awaiting Token Authorization...";
                
                if(await Auth.ResolveIdentifier() && Storage.UserInstance != null)
                {
                    AuthStatus = "Authorized!";
                    Storage.PreviousAuth = true;
                    await Storage.MainVM?.NavigateToPageByViewModelAsync(new ServerSelectionViewModel())!;
                    return;
                }
            }
            
            await Auth.GetIdentifier();
            await Auth.GetOAuth2URL();
        
            if(Storage.PreviousAuth)
            {
                Storage.OAuth2URL = Storage.OAuth2URL.Replace("&prompt=none", "&prompt=consent");
            }
        
            Utility.OpenUrl(Storage.OAuth2URL + "&state=" + Storage.Identifier);

            while (true)
            {
                await Task.Delay(1000);
                if(await Auth.ResolveIdentifier() && Storage.UserInstance != null)
                {
                    AuthStatus = "Authorized!";
                    Storage.PreviousAuth = true;
                    
                    try
                    {
                        await File.WriteAllTextAsync(Path.Combine(Storage.FolderPath, "erm.auth"), Storage.Identifier);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    
                    await Storage.MainVM?.NavigateToPageByViewModelAsync(new ServerSelectionViewModel())!;
                    return;
                }
            }
            
        }
    }

    public override Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}