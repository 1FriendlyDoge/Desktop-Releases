using System;
using System.ComponentModel;
using Avalonia.Controls;

namespace ERM_Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        Environment.Exit(0);
    }
}