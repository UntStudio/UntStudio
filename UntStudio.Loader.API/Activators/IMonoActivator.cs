using System;

namespace UntStudio.Loader.API.Activators;

public interface IMonoActivator
{
    IntPtr Activate(byte[] bytes);
}