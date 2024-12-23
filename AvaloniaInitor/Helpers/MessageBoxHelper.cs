using System;
using System.Threading.Tasks;
using MsBox.Avalonia;

namespace AvaloniaInitor.Helpers;

public static class MessageBoxHelper
{
    public static async Task Error(string message)
    {
        await Console.Error.WriteLineAsync(message);
        await MessageBoxManager.GetMessageBoxStandard("Error", message).ShowAsync();

    }

    public static async Task Ok(string message)
    {
        await MessageBoxManager.GetMessageBoxStandard("Ok", message).ShowAsync();
    }
}
