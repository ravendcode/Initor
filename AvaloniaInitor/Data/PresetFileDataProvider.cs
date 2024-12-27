using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AvaloniaInitor.Models;
using dotenv.net;

namespace AvaloniaInitor.Data;

public interface IPresetFileDataProvider
{
    Task<IEnumerable> GetAllAsync();
    IEnumerable GetAll(Config config);
}

public class PresetFileDataProvider : IPresetFileDataProvider
{
    private readonly string _presetsDir = DotEnv.Read()["APP_SETTINGS_DIR"] +"\\";
    private readonly List<PresetFile> _presetFiles = [];
    private Config? _config;

    public async Task<IEnumerable> GetAllAsync()
    {
        await Task.Delay(100);
        return new List<PresetFile>
        {
            new("Art", true),
            new("Art/Environment", true),
            new("Audio/Sounds", true),
            new("Audio/Music", true),
            new("Resources", false)
        };
    }

    public IEnumerable GetAll(Config config)
    {
        _config = config;
        ProjectNameToPascalCase();
        GenerateDirectories();
        GenerateFiles();
        return _presetFiles;
    }

    private void GenerateDirectories()
    {
        var rawDirectories = Directory.GetDirectories(_presetsDir + _config?.PresetName, "*",
            SearchOption.AllDirectories);

        foreach (var rawDirectory in rawDirectories)
        {
            var directoryProjectName = CreateProjectNamePath(rawDirectory);
            _presetFiles.Add(new PresetFile(directoryProjectName, true));
        }
    }

    private void GenerateFiles()
    {
        var rawFiles = Directory.GetFiles(_presetsDir + _config?.PresetName, "*", SearchOption.AllDirectories);

        foreach (var rawFile in rawFiles)
        {
            var fileProjectName = CreateProjectNamePath(rawFile);
            _presetFiles.Add(new PresetFile(fileProjectName, true, rawFile));
        }
    }

    private string CreateProjectNamePath(string raw)
    {
        return raw.Replace(_presetsDir + _config?.PresetName, _config?.ProjectName)
            .Replace("$ProjectName$", _config!.AssetsProjectName);
    }

    private void ProjectNameToPascalCase()
    {
        var pascalCaseProjectName = "";

        var rawProjectName = Path.GetFileName(_config?.ProjectName!).Replace("-", " ");
        rawProjectName = rawProjectName.Replace("_", " ");
        // var chunks = Path.GetFileName(_config?.ProjectName!).Split();
        var chunks = rawProjectName.Split();
        if (chunks.Length > 1)
        {
            foreach (var chunk in chunks)
            {
                pascalCaseProjectName += char.ToUpper(chunk[0]) + chunk[1..];
            }
        }
        else
        {
            pascalCaseProjectName += chunks[0];
        }
        _config!.AssetsProjectName = pascalCaseProjectName;
    }
}
