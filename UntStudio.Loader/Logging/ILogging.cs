using System;

namespace UntStudio.Loader.Logging
{
    public interface ILogging
    {
        void Log(string message);

        void LogError(Exception ex, string message);
    }
}
