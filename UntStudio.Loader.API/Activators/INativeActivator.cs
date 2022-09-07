using System;
using System.Reflection;

namespace UntStudio.Loader.API.Activators;

public interface INativeActivator
{
    void Activate(IntPtr handle, Assembly assembly);
}
