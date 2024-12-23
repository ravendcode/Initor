using IrrKlang;

namespace AvaloniaInitor.Helpers;

public static class AudioHelper
{
    public static void PlaySound(string soundName)
    {
        var engine = new ISoundEngine();

        // play a sound file
        engine.Play2D($"Audio/Sounds/{soundName}");
    }
}
