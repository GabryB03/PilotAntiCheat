using MetroSuite;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

public partial class MainForm : MetroForm
{
    public MainForm()
    {
        if (!(new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator))
        {
            Process.GetCurrentProcess().Kill();
            return;
        }

        InitializeComponent();
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
        CheckForIllegalCrossThreadCalls = false;

        foreach (AntiTimeModificationFunction func in Enum.GetValues(typeof(AntiTimeModificationFunction)))
        {
            Thread thread = new Thread(() => RunAntiTimeModification(func));
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

         {
            Thread thread = new Thread(() => RunFunctionAntiTimeModification());
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        {
            Thread thread = new Thread(() => RunAntiDebug());
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        {
            Thread thread = new Thread(() => RunAntiBadModules());
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        {
            Thread thread = new Thread(() => RunAntiBadProcesses());
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        {
            Thread thread = new Thread(() => RunClearRAM());
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        // Implement Anti VM.
        // Implement Anti Sandbox (check also window title [#]).
        // Implement Anti Thread Suspend.
        // Implement Anti Thread Termination.
        // Check for bad drivers (interception {mouse.sys, keyboard.sys}, KProcessHacker, DBK64 {Cheat Engine Driver}).
        // https://stackoverflow.com/questions/23327660/how-to-check-whether-a-driver-is-installed
        // https://stackoverflow.com/questions/24874558/how-to-get-a-driver-list-in-windows
        // https://learn.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicecontroller.getdevices?view=dotnet-plat-ext-6.0

        // Useful resources:
        // 0) https://github.com/AdvDebug/AntiCrack-DotNet
        // 1) https://github.com/ExpLife0011/Sagaan-AntiCheat-V2.0
        // 2) https://www.unknowncheats.me/forum/anti-cheat-bypass/291300-sagaan-anticheat-analysis.html
        // 3) https://github.com/mathisvickie/KMAC
        // 4) https://github.com/allogic/KDBG
        // 5) https://github.com/hrt/MouseInjectDetection
        // 6) https://github.com/gmh5225/Detection-CheatEngine
        // 7) https://github.com/sank20144/MNS-System
        // 8) https://github.com/LYingSiMon/al-khaser
        // 9) https://github.com/LYingSiMon/m_anticheat
        // 10) https://reverseengineering.stackexchange.com/questions/2262/how-can-dll-injection-be-detected
        // 11) https://github.com/Soterball/DLLInjectionDetection

        AntiDump.RunAntiDump();
        AntiDebug.HideCurrentThreadFromDebugger();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        Process.GetCurrentProcess().Kill();
    }

    public void RunAntiTimeModification(AntiTimeModificationFunction func)
    {
        AntiDebug.HideCurrentThreadFromDebugger();

        while (true)
        {
            AntiDebug.HideCurrentThreadFromDebugger();

            if (AntiTimeModification.IsTimeModified(func))
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }

    public void RunFunctionAntiTimeModification()
    {
        AntiDebug.HideCurrentThreadFromDebugger();

        while (true)
        {
            Thread.Sleep(500);
            AntiDebug.HideCurrentThreadFromDebugger();

            if (AntiTimeModification.AreTimeFunctionsPatched())
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }

    public void RunAntiDebug()
    {
        AntiDebug.HideCurrentThreadFromDebugger();

        while (true)
        {
            Thread.Sleep(500);
            AntiDebug.HideCurrentThreadFromDebugger();

            if (AntiDebug.IsProcessDebugged())
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }

    public void RunAntiBadModules()
    {
        AntiDebug.HideCurrentThreadFromDebugger();

        while (true)
        {
            Thread.Sleep(1000);
            AntiDebug.HideCurrentThreadFromDebugger();

            if (AntiBadModules.IsBadModuleLoaded())
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }

    public void RunAntiBadProcesses()
    {
        AntiDebug.HideCurrentThreadFromDebugger();

        while (true)
        {
            Thread.Sleep(1000);
            AntiDebug.HideCurrentThreadFromDebugger();

            if (AntiBadProcesses.IsBadProcessRunning())
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }

    public void RunClearRAM()
    {
        AntiDebug.HideCurrentThreadFromDebugger();

        while (true)
        {
            Thread.Sleep(5000);
            AntiDebug.HideCurrentThreadFromDebugger();
            Utils.ClearRAM();
        }
    }
}