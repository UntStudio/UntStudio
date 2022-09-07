using System;

namespace UntStudio.Loader.API.Activators;

public class UnsupportedPluginFrameworkException : Exception
{
    public UnsupportedPluginFrameworkException(string message) : base(message)
    {
    }
    public UnsupportedPluginFrameworkException()
    {
    }
}
