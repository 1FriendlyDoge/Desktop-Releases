using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ERM_Desktop.ViewModels;

namespace ERM_Desktop.Views.Widgets;

public partial class ModerationWidgetView : Window
{
    private readonly HomeViewModel _viewModel = new();

    public ModerationWidgetView()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public ModerationWidgetView(HomeViewModel viewModel)
    {
        _viewModel = viewModel;
        AvaloniaXamlLoader.Load(this);

        if(_viewModel.ModPinned)
        {
            Topmost = true;
        }

        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.ModPinned))
            {
                Topmost = _viewModel.ModPinned;
            }
        };
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if(!_viewModel.ModLocked)
        {
            BeginMoveDrag(e);
        }
    }

    private async void InputElement_OnLostFocusAuto(object? sender, RoutedEventArgs e)
    {
        _viewModel.moderationViewModel!.CompletesOpen = false;
        _viewModel.moderationViewModel!.Focus = false;
        await _viewModel.moderationViewModel!.ForceUserlookup();
    }
    
    private void InputElement_OnGotFocusAuto(object? sender, GotFocusEventArgs e)
    {
        _viewModel.moderationViewModel!.Focus = true;
    }

    private void InputElement_OnPointerPressedId(object? sender, PointerPressedEventArgs e)
    {
        string uid = _viewModel.moderationViewModel!.UserIdResult;
        
        if(uid != "None")
        {
            Application.Current?.Clipboard?.SetTextAsync(uid).Wait();
        }
    }

    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if(e.Key == Key.Back || e.Key == Key.Delete)
        {
            _viewModel.moderationViewModel!.SelectedItem = null;
            
            if(e.KeyModifiers == KeyModifiers.Control)
            {
                _viewModel.moderationViewModel!.UsernameInput = String.Empty;
            }
        }
    }
}