namespace UntStudio.Loader.Logging
{
    internal class ConsoleLogging : ILogging
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
