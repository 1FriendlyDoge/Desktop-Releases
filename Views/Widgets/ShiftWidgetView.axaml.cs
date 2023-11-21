using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ERM_Desktop.ViewModels;

namespace ERM_Desktop.Views.Widgets;

public partial class ShiftWidgetView : Window
{
    private readonly HomeViewModel _viewModel = new();
    
    public ShiftWidgetView()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public ShiftWidgetView(HomeViewModel viewModel)
    {
        _viewModel = viewModel;
        AvaloniaXamlLoader.Load(this);
        
        if(viewModel.ShiftPinned)
        {
            Topmost = true;
        }

        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.ShiftPinned))
            {
                Topmost = _viewModel.ShiftPinned;
            }
        };
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if(!_viewModel.ShiftLocked)
        {
            BeginMoveDrag(e);
        }
    }
}