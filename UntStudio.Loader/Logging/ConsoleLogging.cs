using System;

namespace UntStudio.Loader.Logging
{
    public class ConsoleLogging : ILogging
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void LogError(Exception ex, string message)
        {
            Log($"[ERROR]: {message} Info: {ex}");
        }
    }
}
