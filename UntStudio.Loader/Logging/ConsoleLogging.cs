using System;
using UntStudio.Loader.API.Services;

namespace UntStudio.Loader.Logging;

public class ConsoleLogging : ILogging
{
    public void Log(string message, ConsoleColor color = ConsoleColor.Gray)
    {
        ConsoleColor previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previousColor;
    }

    public void LogWarning(string message)
    {
        Log($"[WRN]: {message}", ConsoleColor.Yellow);
    }

    public void LogException(Exception ex, string message)
    {
        Log($"[EXC]: {message} \nInfo: {ex}", ConsoleColor.Red);
    }
}
