namespace AvaloniaInitor.Models;

public class AppUserSettings
{
    public StartLocation StartLocations { get; set; } = null!;
}

public class StartLocation
{
    public string Unity { get; set; } = null!;
    public string Unreal { get; set; } = null!;
    public string Godot { get; set; } = null!;
}
