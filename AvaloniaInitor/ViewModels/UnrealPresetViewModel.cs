using System;
using System.IO;
using System.Threading.Tasks;
using AvaloniaInitor.Data;
using AvaloniaInitor.Helpers;
using AvaloniaInitor.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaInitor.ViewModels;

public partial class UnrealPresetViewModel : PresetBaseViewModel
{
    [ObservableProperty] private int _maxFps = 60;
    [ObservableProperty] private bool _isMaxFpsSelected = true;
    [ObservableProperty] private bool _isUnrealVersion5;
    [ObservableProperty] private bool _isAppendEditorPerProjectUserSettingsSelected = true;

    public UnrealPresetViewModel(Config config, IPresetFileDataProvider presetFileDataProvider) : base(config,
        presetFileDataProvider)
    {
        Title = config.PresetName;
    }

    public UnrealPresetViewModel()
    {
    }

    protected override async Task CreateAll()
    {
        try
        {
            if (IsMaxFpsSelected)
            {
                await SetFrameRateLimit();
            }
            if (IsAppendEditorPerProjectUserSettingsSelected)
            {
                await AppendEditorPerProjectUserSettings();
            }
        }
        catch (Exception e)
        {
            await MessageBoxHelper.Error(e.Message);
            return;
        }

        await base.CreateAll();
    }

    private async Task AppendEditorPerProjectUserSettings()
    {
        var editorPerProjectUserSettingsFile = Config.ProjectPath + (IsUnrealVersion5
            ? "Saved/Config/WindowsEditor/EditorPerProjectUserSettings.ini"
            : "Saved/Config/Windows/EditorPerProjectUserSettings.ini");
        var srcEditorPerProjectUserSettingsFile =
            Path.Combine(Environment.CurrentDirectory, $"{SettingsDir}/EditorPerProjectUserSettings.ini");
        var srcText = await File.ReadAllLinesAsync(srcEditorPerProjectUserSettingsFile);
        for (var i = 0; i < srcText.Length; i++)
        {
            srcText[i] = srcText[i].Replace("$AssetsProjectName$", Config.AssetsProjectName);
        }
        srcText[^1] += Environment.NewLine;
        await File.AppendAllLinesAsync(editorPerProjectUserSettingsFile, srcText);
    }

    private async Task SetFrameRateLimit()
    {
        var editorUserSettingsFile = Config.ProjectPath + (IsUnrealVersion5
            ? "Saved/Config/WindowsEditor/GameUserSettings.ini"
            : "Saved/Config/Windows/GameUserSettings.ini");
        const string lineContains = "FrameRateLimit=";
        var newFrameRateLimit = $"FrameRateLimit={MaxFps}.000000";
        await FileHelper.ReplaceLineAsync(editorUserSettingsFile, lineContains, newFrameRateLimit);
    }
}
