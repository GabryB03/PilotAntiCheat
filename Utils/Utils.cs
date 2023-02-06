using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime;

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

    public static bool IsFunctionPatched(string library, string functionName)
    {
        IntPtr kernel32 = LoadLibrary(library);
        IntPtr GetProcessId = GetProcAddress(kernel32, functionName);
        byte[] data = new byte[1];
        Marshal.Copy(GetProcessId, data, 0, 1);
        return data[0] == 0xE9;
    }

    public static bool IsFunctionPatched(Type theType, string methodName)
    {
        byte[] data = new byte[1];
        var getMethod = theType.GetMethod(methodName);
        IntPtr targetAddre = getMethod.MethodHandle.GetFunctionPointer();
        Marshal.Copy(targetAddre, data, 0, 1);
        return data[0] == 0x33;
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