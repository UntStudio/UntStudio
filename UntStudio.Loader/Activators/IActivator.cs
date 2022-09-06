using System;

namespace UntStudio.Loader.Activators
{
    public interface IActivator<TArg>
    {
        void Activate(IntPtr handle, TArg arg);
    }
}
