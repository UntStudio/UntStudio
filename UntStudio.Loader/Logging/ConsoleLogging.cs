using System;

namespace UntStudio.Loader.Logging
{
    public class ConsoleLogging : ILogging
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
