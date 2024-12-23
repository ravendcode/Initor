using System;
using System.Threading.Tasks;
using MsBox.Avalonia;

namespace AvaloniaInitor.Helpers;

public static class MessageBoxHelper
{
    public static async Task Error(string message)
    {
        AudioHelper.PlaySound("error.wav");
        await Console.Error.WriteLineAsync(message);
        await MessageBoxManager.GetMessageBoxStandard("Error", message).ShowAsync();

    }

    public static async Task Ok(string message)
    {
        AudioHelper.PlaySound("success.wav");
        await MessageBoxManager.GetMessageBoxStandard("Ok", message).ShowAsync();
    }
}
