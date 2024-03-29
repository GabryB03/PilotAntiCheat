﻿using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime;
using System.Windows.Forms;

public class Utils
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr LoadLibrary(string dllToLoad);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

    [DllImport("psapi.dll")]
    private static extern int EmptyWorkingSet(IntPtr hwProc);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

    [DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern void RtlInitUnicodeString(out UNICODE_STRING DestinationString, string SourceString);

    [DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern void RtlUnicodeStringToAnsiString(out ANSI_STRING DestinationString, UNICODE_STRING UnicodeString, bool AllocateDestinationString);

    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern uint LdrGetDllHandle([MarshalAs(UnmanagedType.LPWStr)] string DllPath, [MarshalAs(UnmanagedType.LPWStr)] string DllCharacteristics, UNICODE_STRING LibraryName, ref IntPtr DllHandle);

    [DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern uint LdrGetProcedureAddress(IntPtr Module, ANSI_STRING ProcedureName, ushort ProcedureNumber, out IntPtr FunctionHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lib);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(IntPtr ProcHandle, IntPtr BaseAddress, byte[] Buffer, uint size, int NumOfBytes);

    private static IntPtr LowLevelGetModuleHandle(string Library)
    {
        IntPtr hModule = IntPtr.Zero;
        UNICODE_STRING UnicodeString = new UNICODE_STRING();
        RtlInitUnicodeString(out UnicodeString, Library);
        LdrGetDllHandle(null, null, UnicodeString, ref hModule);
        return hModule;
    }

    private static IntPtr LowLevelGetProcAddress(IntPtr hModule, string Function)
    {
        IntPtr FunctionHandle = IntPtr.Zero;
        UNICODE_STRING UnicodeString = new UNICODE_STRING();
        ANSI_STRING AnsiString = new ANSI_STRING();
        RtlInitUnicodeString(out UnicodeString, Function);
        RtlUnicodeStringToAnsiString(out AnsiString, UnicodeString, true);
        LdrGetProcedureAddress(hModule, AnsiString, 0, out FunctionHandle);
        return FunctionHandle;
    }

    public static bool IsFunctionPatched(string library, string functionName)
    {
        IntPtr kernel32 = LoadLibrary(library);
        IntPtr GetProcessId = GetProcAddress(kernel32, functionName);
        byte[] data = new byte[1];
        Marshal.Copy(GetProcessId, data, 0, 1);

        IntPtr hModule = LowLevelGetModuleHandle(library);
        IntPtr Function = LowLevelGetProcAddress(hModule, functionName);
        byte[] FunctionBytes = new byte[1];
        Marshal.Copy(Function, FunctionBytes, 0, 1);

        if (FunctionBytes[0] == 0xE9 || FunctionBytes[0] == 0x90)
        {
            return true;
        }

        return data[0] == 0xE9 || data[0] == 0x90;
    }

    public static bool IsFunctionHardPatched(string library, string functionName)
    {
        IntPtr kernel32 = LoadLibrary(library);
        IntPtr GetProcessId = GetProcAddress(kernel32, functionName);
        byte[] data = new byte[1];
        Marshal.Copy(GetProcessId, data, 0, 1);

        IntPtr hModule = LowLevelGetModuleHandle(library);
        IntPtr Function = LowLevelGetProcAddress(hModule, functionName);
        byte[] FunctionBytes = new byte[1];
        Marshal.Copy(Function, FunctionBytes, 0, 1);

        if (FunctionBytes[0] == 0xE9 || FunctionBytes[0] == 0x90 || FunctionBytes[0] == 255)
        {
            return true;
        }

        return data[0] == 0xE9 || data[0] == 0x90 || data[0] == 255;
    }

    public static bool IsFunctionPatched(Type theType, string methodName)
    {
        byte[] data = new byte[1];
        var getMethod = theType.GetMethod(methodName);
        IntPtr targetAddre = getMethod.MethodHandle.GetFunctionPointer();
        Marshal.Copy(targetAddre, data, 0, 1);
        return data[0] == 0x33 || data[0] == 0xE9 || data[0] == 0x90 || data[0] == 255;
    }

    public static string GetPatternFromString(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        string result = "";

        foreach (byte b in bytes)
        {
            if (result == "")
            {
                result = b.ToString("X2");
            }
            else
            {
                result += " " + b.ToString("X2");
            }
        }

        return result;
    }

    public static string GetPatternFromBytes(byte[] bytes)
    {
        string result = "";

        foreach (byte b in bytes)
        {
            if (result == "")
            {
                result = b.ToString("X2");
            }
            else
            {
                result += " " + b.ToString("X2");
            }
        }

        return result;
    }

    public static string FilterString(string str)
    {
        char[] characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        string result = "";

        foreach (char c in str)
        {
            foreach (char s in characters)
            {
                if (c.Equals(s))
                {
                    result += s;
                    break;
                }
            }
        }

        return result.ToLower();
    }

    public static string GetPathFromFileName(string str)
    {
        string newPath = str.ToLower();
        string fileName = System.IO.Path.GetFileName(newPath);
        return newPath.Substring(0, newPath.Length - fileName.Length - 1);
    }

    public static void ClearRAM()
    {
        EmptyWorkingSet(Process.GetCurrentProcess().Handle);
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect(GC.MaxGeneration);
        GC.WaitForPendingFinalizers();
        SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
    }
}