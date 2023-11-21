using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ERM_Desktop.ViewModels;

namespace ERM_Desktop.Views.Widgets;

public partial class BanBolosWidgetView : Window
{
    private readonly HomeViewModel _viewModel = new();
    
    public BanBolosWidgetView()
    {
        InitializeComponent();
    }

    public BanBolosWidgetView(HomeViewModel viewModel)
    {
        _viewModel = viewModel;
        AvaloniaXamlLoader.Load(this);
        
        if(_viewModel.BolosPinned)
        {
            Topmost = true;
        }

        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.BolosPinned))
            {
                Topmost = _viewModel.BolosPinned;
            }
        };
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if(!_viewModel.BolosLocked)
        {
            BeginMoveDrag(e);
        }
    }
}