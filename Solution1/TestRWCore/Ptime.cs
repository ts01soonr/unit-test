using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace TestRWCore
{
    public class Ptime 
    {
        private int processid;

        public Ptime(String pid)
        {
            processid = int.Parse(pid);
        }
        public void CT()
        {
            getCreate(processid);
        }
        public static void getCreate(int pid)
        {
            //Console.WriteLine("Opening PID {0}", pid);

            uint access = 0x1000;
            using (var hproc = OpenProcess(access, false, (uint)pid))
            {
                if (hproc.IsInvalid)
                {
                    Console.WriteLine("OpenProcess failed");
                    //Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                ulong ct, et, kt, ut;
                if (!GetProcessTimes(hproc, out ct, out et, out kt, out ut))
                {
                    Console.WriteLine("GetProcessTimes failed");
                    //Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                //Console.WriteLine("Create time: 0x{0:x} [ {1} ]", ct, DateTime.FromFileTime((long)ct));
                Console.Write("0x{0:x}", ct);
            }
        }
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessTimes(
            SafeProcessHandle hProcess,
            out ulong lpCreationTime,
            out ulong lpExitTime,
            out ulong lpKernelTime,
            out ulong lpUserTime
        );

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern SafeProcessHandle OpenProcess(
            uint dwDesiredAccess,
            bool bInheritHandle,
            uint dwProcessId
            );



    }

}
