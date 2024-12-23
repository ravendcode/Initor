namespace AvaloniaInitor.Models;

public class Config(string? projectPath, string? projectName, string? presetName)
{
    public string? ProjectPath { get; set; } = projectPath;
    public string? ProjectName { get; set; } = projectName;
    public string? PresetName { get; set; } = presetName;
    public string? AssetsProjectName { get; set; }
}
