using System;
using System.Reflection;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AvaloniaInitor.Helpers;

public static class ImageHelper
{
    public static Bitmap LoadFromResource(string resourcePath)
    {
        Uri resourceUri;
        if (!resourcePath.StartsWith("avares://"))
        {
            // var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
            // const string assemblyName = "AvaloniaInitor";
            const string assemblyName = "Initor";
            resourceUri = new Uri($"avares://{assemblyName}/{resourcePath.TrimStart('/')}");
        }
        else
        {
            resourceUri = new Uri(resourcePath);
        }
        return new Bitmap(AssetLoader.Open(resourceUri));
    }

    public static Bitmap LoadFromResource(Uri resourceUri)
    {
        return new Bitmap(AssetLoader.Open(resourceUri));
    }
}
