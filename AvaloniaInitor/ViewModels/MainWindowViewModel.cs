using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using AvaloniaInitor.Data;
using AvaloniaInitor.Helpers;
using AvaloniaInitor.Models;
using AvaloniaInitor.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaInitor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IAppUserSettingsDataProvider _appUserSettingsDataProvider;
    [ObservableProperty] private bool _isPaneOpen = true;
    [ObservableProperty] private string? _projectPath;
    [ObservableProperty] private ListItemTemplate? _selectedListItem;
    [ObservableProperty] private ViewModelBase _currentPage;
    [ObservableProperty] private string? _themeIco;
    private bool _isDarkTheme;
    private Config _config = new(null, "New Project", "Unity");
    private AppUserSettings? _appUserSettings;

    public ObservableCollection<ListItemTemplate> Items { get; }

    public MainWindowViewModel(IAppUserSettingsDataProvider appUserSettingsDataProvider)
    {
        _appUserSettingsDataProvider = appUserSettingsDataProvider;

        _isDarkTheme = Application.Current!.ActualThemeVariant == ThemeVariant.Dark;
        ThemeIco = _isDarkTheme ? ThemeVariantIco.Light : ThemeVariantIco.Dark;

        _currentPage = new UnityPresetViewModel(_config, new PresetFileDataProvider());

        Items =
        [
            new ListItemTemplate(typeof(UnityPresetViewModel), _config,
                ImageHelper.LoadFromResource("Assets/unity.png")),
            new ListItemTemplate(typeof(UnrealPresetViewModel), new Config(null, "New Project", "Unreal"),
                ImageHelper.LoadFromResource("Assets/ue.png")),
            new ListItemTemplate(typeof(GodotPresetViewModel), new Config(null, "New Project", "Godot"),
                ImageHelper.LoadFromResource("Assets/godot.png"))
        ];
        _selectedListItem = Items[0];
    }

#if DEBUG
    public MainWindowViewModel()
    {
        _appUserSettingsDataProvider = new AppUserAppUserSettingsDataProvider();

        _isDarkTheme = Application.Current!.ActualThemeVariant == ThemeVariant.Dark;
        ThemeIco = _isDarkTheme ? ThemeVariantIco.Light : ThemeVariantIco.Dark;

        _currentPage = new UnityPresetViewModel(_config, new PresetFileDataProvider());

        Items =
        [
            new ListItemTemplate(typeof(UnityPresetViewModel), _config,
                ImageHelper.LoadFromResource("Assets/unity.png")),
            new ListItemTemplate(typeof(UnrealPresetViewModel), new Config(null, "New Project", "Unreal"),
                ImageHelper.LoadFromResource("Assets/ue.png")),
            new ListItemTemplate(typeof(GodotPresetViewModel), new Config(null, "New Project", "Godot"),
                ImageHelper.LoadFromResource("Assets/godot.png"))
        ];
        _selectedListItem = Items[0];

    }
#endif

    public async Task LoadAsync()
    {
        _appUserSettings = await _appUserSettingsDataProvider.LoadAsync();
        if (_appUserSettings is null) await MessageBoxHelper.Error("Failed to load settings.");
    }

    [RelayCommand] private void TriggerPane() => IsPaneOpen = !IsPaneOpen;

    [RelayCommand] private void TriggerTheme()
    {
        AudioHelper.PlaySound("click.wav");
        _isDarkTheme = !_isDarkTheme;
        if (_isDarkTheme)
        {
            ThemeIco = ThemeVariantIco.Light;
            Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
        }
        else
        {
            ThemeIco = ThemeVariantIco.Dark;
            Application.Current!.RequestedThemeVariant = ThemeVariant.Light;
        }
    }

    [RelayCommand] private void SettingsPage() => CurrentPage = new AppUserSettingsViewModel(_appUserSettings!, _appUserSettingsDataProvider);

    public async void SelectProject(MainWindow mainWindow)
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(mainWindow);
            var startLocation = _appUserSettings!.StartLocations.GetType().GetProperty(_config.PresetName!)
                ?.GetValue(_appUserSettings.StartLocations);
            Uri.TryCreate("file://" + startLocation, UriKind.Absolute, out var uri);
            var folder = await topLevel!.StorageProvider.TryGetFolderFromPathAsync(uri!);
            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                SuggestedStartLocation = folder,
                Title = $"Open {_config.PresetName!} Project",
                AllowMultiple = false
            });

            if (folders.Count < 1) return;
            ProjectPath = folders[0].Path.ToString().Replace("file:///", "");
            foreach (var item in Items)
            {
                item.Config.ProjectPath = ProjectPath;
                item.Config.ProjectName = folders[0].Name;
            }

            OnSelectedListItemChanged(SelectedListItem);
        }
        catch (Exception e)
        {
            await MessageBoxHelper.Error(e.Message);
        }
    }

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;
        var instance = Activator.CreateInstance(value.ModelType, value.Config, new PresetFileDataProvider());
        _config = value.Config;
        if (instance is null) return;
        // CurrentPreset = (ViewModelBase)instance;
        CurrentPage = (PresetBaseViewModel)instance;
    }
}

public class ListItemTemplate(Type modelType, Config config, Bitmap image)
{
    public string Label { get; } = modelType.Name.Replace("PresetViewModel", string.Empty);
    public Config Config { get; } = config;
    public Type ModelType { get; } = modelType;
    public Bitmap Image { get; } = image;
}

public abstract record ThemeVariantIco
{
    public static string Dark => "\ue330";
    public static string Light => "\ue472";
}
