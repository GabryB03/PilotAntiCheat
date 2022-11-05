using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Diagnostics;

public class ModuleFileName
{
    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool CloseHandle(IntPtr handle);

    [Flags]
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VirtualMemoryOperation = 0x00000008,
        VirtualMemoryRead = 0x00000010,
        VirtualMemoryWrite = 0x00000020,
        DuplicateHandle = 0x00000040,
        CreateProcess = 0x000000080,
        SetQuota = 0x00000100,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        QueryLimitedInformation = 0x00001000,
        Synchronize = 0x00100000,
        PROCESS_QUERY_INFORMATION = 0x0400,
        PROCESS_VM_READ = 0x10
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

    [DllImport("psapi.dll", SetLastError = true)]
    public static extern int GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, int nSize);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpExeName, ref int lpdwSize);

    [DllImport("psapi.dll")]
    public static extern uint GetProcessImageFileName(IntPtr hProcess, [Out] StringBuilder lpImageFileName, [In][MarshalAs(UnmanagedType.U4)] int nSize);

    [DllImport("kernel32.dll")]
    public static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

    public static string GetExecutablePath(int processId)
    {
        try
        {
            Process process = Process.GetProcessById(processId);
            return process.MainModule.FileName;
        }
        catch
        {

        }

        try
        {
            StringBuilder buff = new StringBuilder(1024);

            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    IntPtr hprocess = ModuleFileName.OpenProcess(ModuleFileName.ProcessAccessFlags.PROCESS_QUERY_INFORMATION, false, processId);

                    if (hprocess != IntPtr.Zero)
                    {
                        try
                        {
                            int size = buff.Capacity;

                            if (ModuleFileName.QueryFullProcessImageName(hprocess, 0, buff, ref size))
                            {
                                return buff.ToString();
                            }
                        }
                        finally
                        {
                            ModuleFileName.CloseHandle(hprocess);
                        }
                    }
                }
            }
            catch
            {

            }

            try
            {
                IntPtr hprocess1 = ModuleFileName.OpenProcess(ModuleFileName.ProcessAccessFlags.PROCESS_QUERY_INFORMATION | ModuleFileName.ProcessAccessFlags.PROCESS_VM_READ, false, processId);

                if (hprocess1 != IntPtr.Zero)
                {
                    try
                    {
                        if (ModuleFileName.GetModuleFileNameEx(hprocess1, IntPtr.Zero, buff, (int)buff.Capacity) > 0)
                        {
                            return buff.ToString();
                        }
                    }
                    finally
                    {
                        ModuleFileName.CloseHandle(hprocess1);
                    }
                }
            }
            catch
            {

            }

            try
            {
                IntPtr hprocess2 = ModuleFileName.OpenProcess(ModuleFileName.ProcessAccessFlags.PROCESS_QUERY_INFORMATION, false, processId);

                if (hprocess2 != IntPtr.Zero)
                {
                    try
                    {
                        if (ModuleFileName.GetProcessImageFileName(hprocess2, buff, (int)buff.Capacity) > 0)
                        {
                            string path = buff.ToString();
                            string driveletter = Path.GetPathRoot(path);

                            foreach (string drive in Environment.GetLogicalDrives())
                            {
                                if (ModuleFileName.QueryDosDevice(drive.TrimEnd('\\'), buff, buff.Capacity) > 0)
                                {
                                    if (path.StartsWith(buff.ToString()))
                                    {
                                        path = path.Remove(0, buff.Length);
                                        return drive + path;
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        ModuleFileName.CloseHandle(hprocess2);
                    }
                }
            }
            catch
            {

            }
        }
        catch
        {

        }

        return "";
    }
}