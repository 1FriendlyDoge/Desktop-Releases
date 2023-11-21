using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ERM_Desktop.Views;

public partial class ServerSelectionView : UserControl
{
    public ServerSelectionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}