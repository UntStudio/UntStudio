using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static UntStudio.Loader.External.ExternalAntiDebugCalls.ExternalWindowsStructs;

namespace UntStudio.Loader.External
{
    internal static class ExternalAntiDebugCalls
    {
        [DllImport("ntdll.dll")]
        internal static extern NtStatus NtSetInformationThread(IntPtr processHandle, ThreadInformationClass @class, IntPtr informationHandle, int length);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenThread(ThreadAccess access, bool bInheritHandle, uint id);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr handle);



        internal static void HideThreadsInCurrentThread()
        {
            ProcessThreadCollection currentProcessThreads = Process.GetCurrentProcess().Threads;
            foreach (ProcessThread currentProcessThread in currentProcessThreads)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[GetOSThreads]: thread.Id {0:X}", currentProcessThread.Id);

                IntPtr openHandle = OpenThread(ThreadAccess.SET_INFORMATION, false, (uint)currentProcessThread.Id);

                if (openHandle == IntPtr.Zero)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[GetOSThreads]: skipped thread.Id {0:X}", currentProcessThread.Id);
                    continue;
                }

                if (HideFromDebugger(openHandle))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[GetOSThreads]: thread.Id {0:X} hidden from debbuger.", currentProcessThread.Id);
                }

                CloseHandle(openHandle);
            }
        }

        internal static bool HideFromDebugger(IntPtr threadHandle)
        {
            NtStatus status = NtSetInformationThread(threadHandle, ThreadInformationClass.ThreadHideFromDebugger, IntPtr.Zero, 0);
            if (status == NtStatus.Success)
            {
                return true;
            }

            return false;
        }

        internal static class ExternalWindowsStructs
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
}
