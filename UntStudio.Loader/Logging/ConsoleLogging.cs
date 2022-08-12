using System;

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
        Log($"[WARN]: {message}", ConsoleColor.Yellow);
    }

    public void LogException(Exception ex, string message)
    {
        Log($"[EXCEPTION]: {message} \nInfo: {ex}", ConsoleColor.Red);
    }
}
