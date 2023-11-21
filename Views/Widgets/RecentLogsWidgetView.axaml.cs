using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ERM_Desktop.ViewModels;

namespace ERM_Desktop.Views.Widgets;

public partial class RecentLogsWidgetView : Window
{
    private readonly HomeViewModel _viewModel = new();
    
    public RecentLogsWidgetView()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public RecentLogsWidgetView(HomeViewModel viewModel)
    {
        _viewModel = viewModel;
        AvaloniaXamlLoader.Load(this);
        
        if(_viewModel.RecentPinned)
        {
            Topmost = true;
        }

        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.RecentPinned))
            {
                Topmost = _viewModel.RecentPinned;
            }
        };
    }
    
    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if(!_viewModel.RecentLocked)
        {
            BeginMoveDrag(e);
        }
    }
}