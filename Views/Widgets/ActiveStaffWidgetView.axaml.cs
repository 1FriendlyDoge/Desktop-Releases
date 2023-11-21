using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ERM_Desktop.ViewModels;

namespace ERM_Desktop.Views.Widgets;

public partial class ActiveStaffWidgetView : Window
{
    private readonly HomeViewModel _viewModel = new();
    
    public ActiveStaffWidgetView()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public ActiveStaffWidgetView(HomeViewModel viewModel)
    {
        _viewModel = viewModel;
        AvaloniaXamlLoader.Load(this);
        
        if(_viewModel.ActivePinned)
        {
            Topmost = true;
        }

        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.ActivePinned))
            {
                Topmost = _viewModel.ActivePinned;
            }
        };
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if(!_viewModel.ActiveLocked)
        {
            BeginMoveDrag(e);
        }
    }
}