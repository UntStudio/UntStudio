using System;

namespace UntStudio.Loader.Logging;

public interface ILogging
{
    void Log(string message, ConsoleColor color = ConsoleColor.Gray);

    void LogWarning(string message);

    void LogException(Exception ex, string message);
}
