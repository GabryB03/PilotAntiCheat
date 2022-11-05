using Reloaded.Memory.Sigscan;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

public class AntiBadProcesses
{
    private static string[] blockedPatterns = new string[]
    {
        "73 79 73 63 6F 6E 73 74 2E 73 69 6E 76 61 6C 69 64 69 6E 70 75 74", // Cheat Engine
        "49 6E 74 65 72 6E 61 6C 20 4F 4C 4C 59 44 42 47 20 65 72 72 6F 72", // OLLYDBG

        // 1) List of debuggers to block: x64dbg, IDA, Ghidra, radare, Binary Ninja, GNU Project Debugger, WinDbg, Cutter, EDB (Evan's Debugger),
        // REDasm, Immunity, Hopper, PEBrowseDbg64, RemedyBG, dnSpy, codeclap, Medusa, Binary Workbench, Softice, LLDB, Nirsoft Simple
        // Program Debugger, Data Display Debugger, kdbg debugger, strace, DebugView, Relyze, PE-bear, Bokken, EmilPRO.

        // 2) List of dumpers to block: MegaDumper, ExtremeDumper.

        // 3) List of memory editing softwares to block: ArtMoney, scanmem, GameConqueror, Bit Slicer, Squalr, WeMod, PINCE,
        // CoSMOS, L. Spiro's Memory Hacking Software, Cheat Tool Set, RAM Cheat, HxD, iHaxGamez.

        // 4) List of process manipulation softwares to block: Process Hacker, Process Explorer, Glances, GNOME System Monitor,
        // KSysGuard, SystemExplorer, TCPView, AnVir Task Manager.

        // 5) List of network manipulation/editing softwares to block: Wireshark, Fiddler, Charles, mitmproxy, Burp Suite, Zed Attack Proxy (ZAP),
        // nmap, HTTP Toolkit, Proxyman, Requestly, Progman, Charles Proxy, Deep Packet Inspection, PRTG Monitor, tcpdump, Savvius Omnipeek,
        // Ettercap, Kismet, SmartSniff, EtherApe, http://httpdebugger.com/

        // 6) List of DLL injector softwares to block: OVD Public Injector, Xenos Injector, DLL Injector, DLL Vaccine, Extreme Injector,
        // Auto DLL Injector, Remote Injector DLL, Injector Gadget.

        // 7) List of sandbox softwares to block (including their modules): Sandboxie, Cameyo, Cuckoo Sandbox, Shade Sandbox, 
        // Firejail, Bufferzone, Shadow Defender, Enigma Virtual Box, Deep Freeze, Any.Run, Toolwiz Time Freeze, Shelter, Island,
        // Bubblewrap, ReHIPS, mbox, BufferZone, WinJail, Buster Sandbox Analyzer, runc, Deep Freeze.

        // For DLL Injectors, find a common part that all DLL injectors are using in order to inject a DLL (CreateRemoteThread).
        // Manual Mapping Detection: https://github.com/vmcall/MapDetection
        // Check loaded DLL header first bytes, check if equal (?), {as remote or manual mapped}.
        // Simple anti cheat example: https://github.com/jnastarot/anti-cheat
        // Protect from memory writing/read by other processes, maybe VirtualProtect/VirtualProtectEx, NtSomeThing that can protect memory regions (?).
    };

    private static string[] blockedProcessNames = new string[]
    {
        "Cheat Engine",
        "OLLYDBG"
    };

    private static string[] blockedWindowTitles = new string[]
    {
        "Cheat Engine",
        "OLLYDBG"
    };

    private static FileList[] blockedFileLists = new FileList[]
    {
        new FileList(new string[] { "ced3d9hook64.dll" }), // Cheat Engine
        new FileList(new string[] { "ollydbg.ini", "Cmdline.dll", "register.txt" }) // OLLYDBG
    };

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    private static string GetCaptionOfActiveWindow()
    {
        var strTitle = string.Empty;
        var handle = GetForegroundWindow();
        var intLength = GetWindowTextLength(handle) + 1;
        var stringBuilder = new StringBuilder(intLength);

        if (GetWindowText(handle, stringBuilder, intLength) > 0)
        {
            strTitle = stringBuilder.ToString();
        }

        return strTitle;
    }

    public static bool IsBadProcessRunning()
    {
        if (Utils.IsFunctionPatched("user32.dll", "GetForegroundWindow"))
        {
            return true;
        }

        string actualWindow = GetCaptionOfActiveWindow();
        string filteredWindow = Utils.FilterString(actualWindow);

        foreach (string windowTitle in blockedWindowTitles)
        {
            string newWindowTitle = Utils.FilterString(windowTitle);

            if (filteredWindow.Contains(newWindowTitle))
            {
                return true;
            }
        }

        foreach (Process process in Process.GetProcesses())
        {
            try
            {
                if (process.MainWindowHandle != null && process.MainWindowHandle != new IntPtr(-1) && process.MainWindowHandle != IntPtr.Zero && process.Id != Process.GetCurrentProcess().Id && !Utils.FilterString(process.MainWindowTitle).Equals(""))
                {
                    if (process.MainWindowTitle == actualWindow)
                    {
                        string filteredProcessName = Utils.FilterString(process.ProcessName);

                        foreach (string processName in blockedProcessNames)
                        {
                            string newProcessName = Utils.FilterString(processName);

                            if (processName.Contains(newProcessName))
                            {
                                return true;
                            }
                        }

                        foreach (FileList fileList in blockedFileLists)
                        {
                            int existsAll = 0;

                            foreach (string theFile in fileList.List)
                            {
                                foreach (string file in System.IO.Directory.GetFiles(Utils.GetPathFromFileName(ModuleFileName.GetExecutablePath(process.Id))))
                                {
                                    if (Utils.FilterString(theFile).Equals(Utils.FilterString(System.IO.Path.GetFileName(file))))
                                    {
                                        existsAll++;
                                        break;
                                    }
                                }
                            }

                            if (existsAll == fileList.List.Length)
                            {
                                return true;
                            }
                        }

                        Scanner scanner = new Scanner(process, process.MainModule);

                        foreach (string pattern in blockedPatterns)
                        {
                            var result = scanner.FindPattern(pattern);

                            if (result.Found)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        return false;
    }
}