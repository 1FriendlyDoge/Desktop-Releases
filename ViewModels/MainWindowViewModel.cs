using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ERM_Desktop.Services;

namespace ERM_Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private PageViewModel? _currentViewModel;
    
    public WindowTransparencyLevel TransparencyLevel => !Storage.ExperimentalMode ? WindowTransparencyLevel.None : RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? WindowTransparencyLevel.AcrylicBlur : WindowTransparencyLevel.None;
    public IBrush BackgroundBrush => !Storage.ExperimentalMode ? new SolidColorBrush(Color.FromRgb(38, 39, 44)) : RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Brushes.Transparent : new SolidColorBrush(Color.FromRgb(38, 39, 44));
    public Thickness LogoMargin => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new Thickness(5, 0, 0, 0) : new Thickness(100, 0, 0, 0);

    public readonly PageViewModel[] viewModels = { new AuthViewModel(), new ServerSelectionViewModel(), new HomeViewModel() };

    public async Task NavigateToPageByViewModelAsync(PageViewModel vm)
    {
        if (CurrentViewModel != null)
        {
            await CurrentViewModel.OnNavigatedFromAsync();
        }

        CurrentViewModel = vm;

        await CurrentViewModel.OnNavigatedToAsync();
    }

    public async Task NavigateToPageByNameAsync(string name)
    {
        foreach (var vm in viewModels)
        {
            if (vm.GetType().Name == name)
            {
                await NavigateToPageByViewModelAsync(vm);

                break;
            }
        }
    }
    
    public MainWindowViewModel()
    {
        Storage.MainVM = this;

        CurrentViewModel = viewModels[0];

        async void Action()
        {
            await NavigateToPageByViewModelAsync(viewModels[0]);
        }

        Task.Run(async () =>
        {
            await Storage.HubConnection.StartAsync();
        }).ConfigureAwait(false);
        
        Dispatcher.UIThread.Post(Action);
    }
}