using System;
using System.Threading.Tasks;
using AvaloniaInitor.Data;
using AvaloniaInitor.Helpers;
using AvaloniaInitor.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using dotenv.net;

namespace AvaloniaInitor.ViewModels;

public partial class UnityPresetViewModel : PresetBaseViewModel
{
    [ObservableProperty] private bool _isAddUnityPluginsSelected = true;
    [ObservableProperty] private bool _isRootNamespaceSelected = true;
    [ObservableProperty] private bool _isDoNotReloadDomainOrSceneSelected = true;
    protected override string PluginsDirUserSettings { get; set; } = DotEnv.Read()["UNITY_PLUGINS_DIR_NAME"];

    public UnityPresetViewModel(Config config, IPresetFileDataProvider presetFileDataProvider) : base(config,
        presetFileDataProvider)
    {
        Title = config.PresetName;
    }

    public UnityPresetViewModel()
    {
    }

    protected override async Task CreateAll()
    {
        try
        {
            if (IsRootNamespaceSelected)
            {
                await GenerateUnityRootNamespace();
            }

            if (IsDoNotReloadDomainOrSceneSelected)
            {
                await SetUnityDoNotReloadDomainOrScene();
            }

            if (IsAddUnityPluginsSelected)
            {
                await AddPlugins();
            }
        }
        catch (Exception e)
        {
            await MessageBoxHelper.Error(e.Message);
            return;
        }

        await base.CreateAll();
    }

    private async Task GenerateUnityRootNamespace()
    {
        var editorSettingsFile = Config.ProjectPath + "ProjectSettings/EditorSettings.asset";
        const string lineContains = "m_ProjectGenerationRootNamespace:";
        var newRootNamespace = "  m_ProjectGenerationRootNamespace: " + AssetsProjectName;
        await FileHelper.ReplaceLineAsync(editorSettingsFile, lineContains, newRootNamespace);
    }

    private async Task SetUnityDoNotReloadDomainOrScene()
    {
        var editorSettingsFile = Config.ProjectPath + "ProjectSettings/EditorSettings.asset";
        const string lineContains = "m_EnterPlayModeOptions:";
        const string newEnterPlayMode = "  m_EnterPlayModeOptions: 3";
        await FileHelper.ReplaceLineAsync(editorSettingsFile, lineContains, newEnterPlayMode);
    }
}
