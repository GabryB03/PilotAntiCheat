using System.Diagnostics;

public class AntiHooks
{
    public static bool IsHookPresent()
    {
        if (Utils.IsFunctionHardPatched("kernel32.dll", "IsDebuggerPresent"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "CheckRemoteDebuggerPresent"))
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

        if (Utils.IsFunctionHardPatched("ntdll.dll", "NtWriteVirtualMemory"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "NtReadVirtualMemory"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "NtAllocateVirtualMemory"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "NtQueryInformationProcess"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "NtClose"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "NtRemoveProcessDebug"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "NtSetInformationDebugObject"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "NtQuerySystemInformation"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "WriteProcessMemory"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "ReadProcessMemory"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "VirtualProtect"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "VirtualProtectEx"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "VirtualAlloc"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "VirtualAllocEx"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "OpenThread"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "NtSetInformationThread"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "GetCurrentThread"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "GetThreadContext"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "OutputDebugStringA"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "GetTickCount"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "GetTickCount64"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("winmm.dll", "timeGetTime"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "QueryPerformanceCounter"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("kernel32.dll", "SetHandleInformation"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("user32.dll", "FindWindowW"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("user32.dll", "FindWindowA"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("user32.dll", "FindWindowExW"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("user32.dll", "FindWindowExA"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("user32.dll", "GetForegroundWindow"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("user32.dll", "GetWindowTextLengthA"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("user32.dll", "GetWindowTextA"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("user32.dll", "BlockInput"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "NtGetContextThread"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("win32u.dll", "NtUserBlockInput"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("win32u.dll", "NtUserFindWindowEx"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("win32u.dll", "NtUserQueryWindow"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("win32u.dll", "NtUserGetForegroundWindow"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "IsDebuggerPresent"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "CheckRemoteDebuggerPresent"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("kernelbase.dll", "CloseHandle"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "WriteProcessMemory"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "ReadProcessMemory"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "VirtualProtect"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "VirtualProtectEx"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "VirtualAlloc"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "VirtualAllocEx"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "OpenThread"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "GetCurrentThread"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "GetThreadContext"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "OutputDebugStringA"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "GetTickCount"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "GetTickCount64"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "QueryPerformanceCounter"))
        {
            return true;
        }

        if (Utils.IsFunctionPatched("kernelbase.dll", "SetHandleInformation"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "RtlInitUnicodeString"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "RtlUnicodeStringToAnsiString"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "LdrGetDllHandle"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "LdrGetProcedureAddress"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "LoadLibraryA"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "LoadLibraryW"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "LoadLibraryA"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "LoadLibraryW"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("ntdll.dll", "LdrLoadDll"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernel32.dll", "CreateThread"))
        {
            return true;
        }

        if (Utils.IsFunctionHardPatched("kernelbase.dll", "CreateThread"))
        {
            return true;
        }

        return false;
    }
}