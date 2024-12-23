using System;
using System.IO;
using System.Threading.Tasks;
using AvaloniaInitor.Data;
using AvaloniaInitor.Helpers;
using AvaloniaInitor.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using dotenv.net;

namespace AvaloniaInitor.ViewModels;

public partial class GodotPresetViewModel : PresetBaseViewModel
{
    [ObservableProperty] private bool _isAddGodotPluginsSelected = true;
    [ObservableProperty] private bool _isAppendProjectSelected = true;
    protected override string PluginsDirUserSettings { get; set; } = DotEnv.Read()["GODOT_PLUGINS_DIR_NAME"];

    public GodotPresetViewModel(Config config, IPresetFileDataProvider presetFileDataProvider) : base(config,
        presetFileDataProvider)
    {
        Title = config.PresetName;
    }

    public GodotPresetViewModel()
    {
    }

    protected override async Task CreateAll()
    {
        try
        {
            if (IsAppendProjectSelected)
            {
                await AppendProjectSelected();
            }

            if (IsAddGodotPluginsSelected)
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

    private async Task AppendProjectSelected()
    {
        var srcProjectGodotFile = Path.Combine(Environment.CurrentDirectory, $"{SettingsDir}/project.godot");
        var destProjectGodotFile = Config.ProjectPath + "project.godot";
        var srcText = await File.ReadAllLinesAsync(srcProjectGodotFile);
        srcText[0] = Environment.NewLine + srcText[0];
        await File.AppendAllLinesAsync(destProjectGodotFile, srcText);
    }
}
