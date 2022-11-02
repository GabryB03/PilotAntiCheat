using MetroSuite;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class AntiDebug
{
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsDebuggerPresent();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern int OutputDebugString(string str);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool CloseHandle(IntPtr handle);

    [DllImport("ntdll.dll", SetLastError = true, ExactSpelling = true)]
    private static extern NtStatus NtQueryInformationProcess([In] IntPtr ProcessHandle, [In] PROCESSINFOCLASS ProcessInformationClass, out IntPtr ProcessInformation, [In] int ProcessInformationLength, [Optional] out int ReturnLength);

    [DllImport("ntdll.dll", SetLastError = true, ExactSpelling = true)]
    private static extern NtStatus NtClose([In] IntPtr Handle);

    [DllImport("ntdll.dll", SetLastError = true, ExactSpelling = true)]
    private static extern NtStatus NtRemoveProcessDebug(IntPtr ProcessHandle, IntPtr DebugObjectHandle);

    [DllImport("ntdll.dll", SetLastError = true, ExactSpelling = true)]
    private static extern NtStatus NtSetInformationDebugObject([In] IntPtr DebugObjectHandle, [In] DebugObjectInformationClass DebugObjectInformationClass, [In] IntPtr DebugObjectInformation, [In] int DebugObjectInformationLength, [Out][Optional] out int ReturnLength);

    [DllImport("ntdll.dll", SetLastError = true, ExactSpelling = true)]
    private static extern NtStatus NtQuerySystemInformation([In] SYSTEM_INFORMATION_CLASS SystemInformationClass, IntPtr SystemInformation, [In] int SystemInformationLength, [Out][Optional] out int ReturnLength);

    [DllImport("ntdll.dll")]
    private static extern NtStatus NtSetInformationThread(IntPtr ThreadHandle, ThreadInformationClass ThreadInformationClass, IntPtr ThreadInformation, int ThreadInformationLength);

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

    public static bool IsProcessDebugged()
    {
        if (Debugger.IsAttached)
        {
            return true;
        }

        if (IsDebuggerPresent())
        {
            return true;
        }

        if (Debugger.IsLogging())
        {
            return true;
        }

        if (string.Compare(Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING"), "1", StringComparison.Ordinal) == 0)
        {
            return true;
        }

        if (Process.GetCurrentProcess().Handle == IntPtr.Zero)
        {
            return true;
        }

        if (OutputDebugString("") > IntPtr.Size)
        {
            return true;
        }

        try
        {
            CloseHandle(IntPtr.Zero);
        }
        catch
        {
            return true;
        }

        if (CheckRemoteDebugger())
        {
            return true;
        }

        if (CheckDebugPort())
        {
            return true;
        }

        if (CheckKernelDebugInformation())
        {
            return true;
        }

        if (Utils.IsFunctionPatched("kernel32.dll", "IsDebuggerPresent"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("kernel32.dll", "CheckRemoteDebuggerPresent"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched(typeof(Debugger), "get_IsAttached"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("kernel32.dll", "CloseHandle"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("ntdll.dll", "NtQueryInformationProcess"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("ntdll.dll", "NtClose"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("ntdll.dll", "NtRemoveProcessDebug"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("ntdll.dll", "NtSetInformationDebugObject"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("ntdll.dll", "NtQuerySystemInformation"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("kernel32.dll", "WriteProcessMemory"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("kernel32.dll", "ReadProcessMemory"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("kernel32.dll", "OpenThread"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("ntdll.dll", "NtSetInformationThread"))
        {
            return true;
        }

        return false;
    }

    private static bool CheckRemoteDebugger()
    {
        var isDebuggerPresent = false;
        var bApiRet = CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);

        return bApiRet && isDebuggerPresent;
    }

    private static bool CheckDebugPort()
    {
        IntPtr DebugPort = new IntPtr(0);
        int ReturnLength;

        unsafe
        {
            if (NtQueryInformationProcess(Process.GetCurrentProcess().Handle, PROCESSINFOCLASS.ProcessDebugPort, out DebugPort, Marshal.SizeOf(DebugPort), out ReturnLength) == NtStatus.Success)
            {
                return DebugPort == new IntPtr(-1);
            }
        }

        return false;
    }

    private static bool CheckKernelDebugInformation()
    {
        SYSTEM_KERNEL_DEBUGGER_INFORMATION pSKDI;
        int retLength;

        unsafe
        {
            if (NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemKernelDebuggerInformation, new IntPtr(&pSKDI), Marshal.SizeOf(pSKDI), out retLength) == NtStatus.Success)
            {
                return pSKDI.KernelDebuggerEnabled && !pSKDI.KernelDebuggerNotPresent;
            }
        }

        return false;
    }

    public static void HideCurrentThreadFromDebugger()
    {
        int currentThreadID = AppDomain.GetCurrentThreadId();
        ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;

        foreach (ProcessThread thread in currentThreads)
        {
            if (thread.Id == currentThreadID)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SET_INFORMATION, false, (uint)thread.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                NtSetInformationThread(pOpenThread, ThreadInformationClass.ThreadHideFromDebugger, IntPtr.Zero, 0);
                CloseHandle(pOpenThread);
                break;
            }
        }
    }
}