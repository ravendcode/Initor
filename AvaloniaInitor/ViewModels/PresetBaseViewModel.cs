using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AvaloniaInitor.Data;
using AvaloniaInitor.Helpers;
using AvaloniaInitor.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using dotenv.net;

namespace AvaloniaInitor.ViewModels;

public partial class PresetBaseViewModel : ViewModelBase
{
    // protected const string SettingsDir = "Settings";
    protected static readonly string SettingsDir = DotEnv.Read()["APP_SETTINGS_DIR"];
    protected readonly Config Config;
    private readonly IPresetFileDataProvider _presetFileDataProvider;
    [ObservableProperty] private PresetFile? _selectedFile;
    [ObservableProperty] private bool _isProjectSelected;

    protected virtual string PluginsDirUserSettings { get; set; } = "Plugins";
    public string? Title { get; set; }
    public string? AssetsProjectName { get; set; }
    public ObservableCollection<PresetFile> PresetFiles { get; } = [];


    protected PresetBaseViewModel(Config config, IPresetFileDataProvider presetFileDataProvider)
    {
        Config = config;
        _presetFileDataProvider = presetFileDataProvider;
        Title = Config.PresetName;
        if (Config.ProjectPath != null)
            _isProjectSelected = true;
        Load();
    }

    protected PresetBaseViewModel()
    {
        Config = new Config(null, "Test Project", "Unity");
        _presetFileDataProvider = new PresetFileDataProvider();
        Title = Config.PresetName;
        if (Config.ProjectPath != null)
            _isProjectSelected = true;
        Load();
    }

    protected void Load()
    {
        if (PresetFiles.Any()) return;
        var presetFiles = _presetFileDataProvider.GetAll(Config);
        foreach (PresetFile presetFile in presetFiles)
        {
            PresetFiles.Add(presetFile);
        }

        AssetsProjectName = Config.AssetsProjectName;
    }

    [RelayCommand] protected virtual async Task CreateAll()
    {
        await GenerateFilesAndDirectories();
    }

    protected async Task AddPlugins()
    {
        var srcPluginsDir =
            new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, $"{SettingsDir}/{PluginsDirUserSettings}"));
        var destPluginsDir = new DirectoryInfo(Config.ProjectPath!);
        await Task.Run(() => FileHelper.CopyAllFiles(srcPluginsDir, destPluginsDir));
    }

    private string DeleteNameProjectRoot(string presetFileName)
    {
        return Regex.Replace(presetFileName, "^" + Config.ProjectName!, "")[1..];
    }

    private async Task GenerateFilesAndDirectories()
    {
        try
        {
            var projectDirectoryInfo = new DirectoryInfo(Config.ProjectPath!);

            foreach (var presetFile in PresetFiles.Where(file => file.IsSelected))
            {
                if (presetFile.SourceFile != null)
                {
                    var sourceFile = Path.Combine(Environment.CurrentDirectory, presetFile.SourceFile);
                    var destFile = Config.ProjectPath + DeleteNameProjectRoot(presetFile.Name);
                    File.Copy(sourceFile, destFile, true);
                }
                else
                {
                    var newDirectory = DeleteNameProjectRoot(presetFile.Name);
                    projectDirectoryInfo.CreateSubdirectory(newDirectory);
                }
            }

            await MessageBoxHelper.Ok("Successfully completed!");
        }
        catch (Exception e)
        {
            await MessageBoxHelper.Error(e.Message);
        }
    }
}
