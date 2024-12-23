using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaInitor.ViewModels;

namespace AvaloniaInitor.Views;

public partial class AppUserSettingsView : UserControl
{
    public AppUserSettingsView()
    {
        InitializeComponent();
        // DataContext = new SettingsViewModel();
    }

    private void ButtonUnityStartLocation_OnClick(object? sender, RoutedEventArgs e)
    {
        (DataContext as AppUserSettingsViewModel)?.SelectStartLocation(this, EStartLocation.Unity);
    }

    private void ButtonUnrealStartLocation_OnClick(object? sender, RoutedEventArgs e)
    {
        (DataContext as AppUserSettingsViewModel)?.SelectStartLocation(this, EStartLocation.Unreal);
    }

    private void ButtonGodotStartLocation_OnClick(object? sender, RoutedEventArgs e)
    {
        (DataContext as AppUserSettingsViewModel)?.SelectStartLocation(this, EStartLocation.Godot);
    }
}
