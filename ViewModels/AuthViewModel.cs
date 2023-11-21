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
    
    private readonly Auth _auth = new Auth();

    public override async Task OnNavigatedToAsync()
    {
        if(!Storage.OAuth2Loaded)
        {
            Storage.OAuth2Loaded = true;
            await _auth.GetIdentifier();
            await _auth.GetOAuth2URL();
            
            if(Storage.PreviousAuth)
            {
                _auth.OAuth2URL = _auth.OAuth2URL.Replace("&prompt=none", "&prompt=consent");
            }
            
            Storage.PreviousAuth = true;
            
            Utility.OpenUrl(_auth.OAuth2URL + "&state=" + Storage.Identifier);

            while (true)
            {
                await Task.Delay(1000);
                if(await _auth.ResolveIdentifier() && Storage.UserInstance != null)
                {
                    AuthStatus = $"Authorized!";
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