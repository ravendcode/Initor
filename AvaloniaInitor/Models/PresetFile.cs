namespace AvaloniaInitor.Models;

// public record PresetFile(string Name, bool IsSelected);

public class PresetFile(string name, bool isSelected, string? sourceFile = null)
{
    public string Name { get; set; } = name;
    public bool IsSelected { get; set; } = isSelected;

    public string? SourceFile { get; set; } = sourceFile;

    public override string ToString()
    {
        return nameof(PresetFile) + " { " + nameof(Name) + " = " + Name + ", IsSelected = " + IsSelected +
               ", SourceFile = " + (SourceFile ?? "null") + " }";
    }
}
