using System.Runtime.InteropServices;
using System;
using System.Text;

public class Utils
{
    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string dllToLoad);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

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
}