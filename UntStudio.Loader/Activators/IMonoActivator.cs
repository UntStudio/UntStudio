using System;

namespace UntStudio.Loader.Activators
{
    public interface IMonoActivator
    {
        IntPtr Activate(byte[] bytes);
    }
}