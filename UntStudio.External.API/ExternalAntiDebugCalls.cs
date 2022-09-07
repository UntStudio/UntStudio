using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static UntStudio.External.API.ExternalAntiDebugCalls.ExternalWindowsStructs;

namespace UntStudio.External.API;

public static class ExternalAntiDebugCalls
{
    [DllImport("ntdll.dll")]
    internal static extern NtStatus NtSetInformationThread(IntPtr processHandle, ThreadInformationClass @class, IntPtr informationHandle, int length);

    [DllImport("kernel32.dll")]
    internal static extern IntPtr OpenThread(ThreadAccess access, bool bInheritHandle, uint id);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool CloseHandle(IntPtr handle);



    public static void HideThreadsInCurrentThread()
    {
        ProcessThreadCollection currentProcessThreads = Process.GetCurrentProcess().Threads;
        foreach (ProcessThread currentProcessThread in currentProcessThreads)
        {
            IntPtr openHandle = OpenThread(ThreadAccess.SET_INFORMATION, false, (uint)currentProcessThread.Id);
            if (openHandle == IntPtr.Zero)
            {
                continue;
            }

            HideFromDebugger(openHandle);
            CloseHandle(openHandle);
        }
    }

    public static bool HideFromDebugger(IntPtr threadHandle)
    {
        NtStatus status = NtSetInformationThread(threadHandle, ThreadInformationClass.ThreadHideFromDebugger, IntPtr.Zero, 0);
        if (status == NtStatus.Success)
        {
            return true;
        }

        return false;
    }

    public static class ExternalWindowsStructs
    {
        internal enum ThreadInformationClass
        {
            ThreadHideFromDebugger = 17,
        }

        [Flags]
        internal enum ThreadAccess : int
        {
            SET_INFORMATION = (0x0020),
        }

        internal enum NtStatus : uint
        {
            Success = 0x00000000,
        }
    }
}
