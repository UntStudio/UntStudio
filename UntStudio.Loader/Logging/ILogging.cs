using System;

namespace UntStudio.Loader.Logging
{
    public interface ILogging
    {
        void Log(string message);

        void LogException(Exception ex, string message);
    }
}
