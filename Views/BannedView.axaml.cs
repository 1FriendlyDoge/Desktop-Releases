using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ERM_Desktop.Views;

public partial class BannedView : UserControl
{
    public BannedView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}