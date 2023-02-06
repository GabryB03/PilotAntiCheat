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

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

    [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetCurrentThread();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetThreadContext(IntPtr hThread, ref CONTEXT Context);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint GetTickCount();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern void OutputDebugStringA(string Text);

    private static long CONTEXT_DEBUG_REGISTERS = 0x00010000L | 0x00000010L;

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

        if (CheckDebugObjectHandle())
        {
            return true;
        }

        if (AntiDebugAttach())
        {
            return true;
        }

        if (CheckHardwareRegistersBreakpoint())
        {
            return true;
        }

        if (GetTickCountAntiDebug())
        {
            return true;
        }

        if (OutputDebugStringAntiDebug())
        {
            return true;
        }

        OllyDbgFormatStringExploit();

        return false;
    }

    private static bool AntiDebugAttach()
    {
        IntPtr NtdllModule = GetModuleHandle("ntdll.dll");
        IntPtr DbgUiRemoteBreakinAddress = GetProcAddress(NtdllModule, "DbgUiRemoteBreakin");
        IntPtr DbgBreakPointAddress = GetProcAddress(NtdllModule, "DbgBreakPoint");
        byte[] Int3InvaildCode = { 0xCC };
        byte[] RetCode = { 0xC3 };
        bool Status = WriteProcessMemory(Process.GetCurrentProcess().Handle, DbgUiRemoteBreakinAddress, Int3InvaildCode, 1, out _);
        bool Status2 = WriteProcessMemory(Process.GetCurrentProcess().Handle, DbgBreakPointAddress, RetCode, 1, out _);

        if (Status && Status2)
        {
            return false;
        }

        return true;
    }

    private static bool CheckHardwareRegistersBreakpoint()
    {
        CONTEXT Context = new CONTEXT();
        Context.ContextFlags = CONTEXT_DEBUG_REGISTERS;

        if (GetThreadContext(GetCurrentThread(), ref Context))
        {
            if ((Context.Dr1 != 0x00 || Context.Dr2 != 0x00 || Context.Dr3 != 0x00 || Context.Dr4 != 0x00 || Context.Dr5 != 0x00 || Context.Dr6 != 0x00 || Context.Dr7 != 0x00))
            {
                return false;
            }
        }

        return true;
    }

    private static bool GetTickCountAntiDebug()
    {
        uint Start = GetTickCount();
        return (GetTickCount() - Start) > 0x10;
    }

    private static bool OutputDebugStringAntiDebug()
    {
        OutputDebugString("just testing some stuff...");
        OutputDebugStringA("just testing some stuff...");

        if (Marshal.GetLastWin32Error() == 0)
        {
            return true;
        }

        return false;
    }

    private static void OllyDbgFormatStringExploit()
    {
        OutputDebugStringA("%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s");
    }

    private static bool CheckRemoteDebugger()
    {
        var isDebuggerPresent = false;
        var bApiRet = CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);

        return bApiRet && isDebuggerPresent;
    }

    private static bool CheckDebugObjectHandle()
    {
        IntPtr hDebugObject = IntPtr.Zero;
        uint Size = sizeof(uint);

        if (Environment.Is64BitProcess)
        {
            Size = sizeof(uint) * 2;
        }

        NtQueryInformationProcess(Process.GetCurrentProcess().Handle, PROCESSINFOCLASS.ProcessDebugObjectHandle, out hDebugObject, (int)Size, out _);

        if (hDebugObject != IntPtr.Zero)
        {
            return true;
        }

        return false;
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