using System;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaInitor.Data;
using AvaloniaInitor.Helpers;
using AvaloniaInitor.Models;
using AvaloniaInitor.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaInitor.ViewModels;

public partial class AppUserSettingsViewModel : ViewModelBase
{
    private readonly AppUserSettings _appUserSettings;
    private readonly IAppUserSettingsDataProvider _appUserSettingsDataProvider;
    [ObservableProperty] private string? _unityStartLocation;
    [ObservableProperty] private string? _unrealStartLocation;
    [ObservableProperty] private string? _godotStartLocation;
    public string Title => "Settings";

    public AppUserSettingsViewModel(AppUserSettings appUserSettings, IAppUserSettingsDataProvider appUserSettingsDataProvider)
    {
        _appUserSettings = appUserSettings;
        _appUserSettingsDataProvider = appUserSettingsDataProvider;
        UnityStartLocation = _appUserSettings.StartLocations.Unity;
        UnrealStartLocation = _appUserSettings.StartLocations.Unreal;
        GodotStartLocation = _appUserSettings.StartLocations.Godot;
    }

#if DEBUG
    public AppUserSettingsViewModel()
    {
        _appUserSettingsDataProvider = new AppUserSettingsDataProvider();
        _appUserSettings = new AppUserSettings
        {
            StartLocations = null!
        };
    }
#endif

    partial void OnUnityStartLocationChanged(string? value)
    {
        _appUserSettings.StartLocations.Unity = value!;
        _appUserSettingsDataProvider.Store(_appUserSettings);
    }

    partial void OnUnrealStartLocationChanged(string? value)
    {
        _appUserSettings.StartLocations.Unreal = value!;
        _appUserSettingsDataProvider.Store(_appUserSettings);
    }

    partial void OnGodotStartLocationChanged(string? value)
    {
        _appUserSettings.StartLocations.Godot = value!;
        _appUserSettingsDataProvider.Store(_appUserSettings);
    }


    public async void SelectStartLocation(AppUserSettingsView settingsView, EStartLocation eStartLocation)
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(settingsView);
            var folders = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = $"Open {eStartLocation} Start Location",
                AllowMultiple = false
            });

            if (folders.Count < 1) return;
            var folder = folders[0].Path.ToString().Replace("file:///", "");
            switch (eStartLocation)
            {
                case EStartLocation.Unity:
                    _appUserSettings.StartLocations.Unity = folder;
                    UnityStartLocation = folder;
                    break;
                case EStartLocation.Unreal:
                    _appUserSettings.StartLocations.Unreal = folder;
                    UnrealStartLocation = folder;
                    break;
                case EStartLocation.Godot:
                    _appUserSettings.StartLocations.Godot = folder;
                    GodotStartLocation = folder;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eStartLocation), eStartLocation, null);
            }
        }
        catch (Exception e)
        {
            await MessageBoxHelper.Error(e.Message);
        }
    }
}

public enum EStartLocation
{
    Unity,
    Unreal,
    Godot
}
