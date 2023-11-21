using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ERM_Desktop.ViewModels;

namespace ERM_Desktop.Views.Widgets;

public partial class SearchWidgetView : Window
{
    private readonly HomeViewModel _viewModel = new();
    
    public SearchWidgetView()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public SearchWidgetView(HomeViewModel viewModel)
    {
        _viewModel = viewModel;
        AvaloniaXamlLoader.Load(this);
        
        if(_viewModel.SearchPinned)
        {
            Topmost = true;
        }

        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.SearchPinned))
            {
                Topmost = _viewModel.SearchPinned;
            }
        };
    }
    
    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if(!_viewModel.SearchLocked)
        {
            BeginMoveDrag(e);
        }
    }
}