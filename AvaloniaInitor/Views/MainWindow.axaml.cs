using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaInitor.Helpers;
using AvaloniaInitor.ViewModels;

namespace AvaloniaInitor.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => { await (DataContext as MainWindowViewModel)?.LoadAsync()!; };
    }

    private void ButtonSelectProject_OnClick(object? sender, RoutedEventArgs e)
    {
        (DataContext as MainWindowViewModel)?.SelectProject(this);
    }
}
