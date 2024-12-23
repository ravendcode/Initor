using System.IO;
using System.Threading.Tasks;

namespace AvaloniaInitor.Helpers;

public static class FileHelper
{
    public static async Task ReplaceLineAsync(string sourceFile, string lineContains, string newLine)
    {
        var text = await File.ReadAllLinesAsync(sourceFile);
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i].Contains(lineContains))
            {
                text[i] = newLine;
                break;
            }
        }

        await File.WriteAllLinesAsync(sourceFile, text);
    }

    public static void CopyAllFiles(DirectoryInfo source, DirectoryInfo destination)
    {
        Directory.CreateDirectory(destination.FullName);
        foreach (var file in source.GetFiles())
        {
            file.CopyTo(Path.Combine(destination.FullName, file.Name), true);
        }
        foreach (var directory in source.GetDirectories())
        {
            var subdirectory = destination.CreateSubdirectory(directory.Name);
            CopyAllFiles(directory, subdirectory);
        }
    }
}
