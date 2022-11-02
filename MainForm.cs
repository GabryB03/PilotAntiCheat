using MetroSuite;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

public partial class MainForm : MetroForm
{
    public MainForm()
    {
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

        // Implement Anti VM.
        // Implement Anti Sandbox (check also window title [#]).

        AntiDump.RunAntiDump();
        AntiDebug.HideCurrentThreadFromDebugger();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        Process.GetCurrentProcess().Kill();
    }

    public void RunAntiTimeModification(AntiTimeModificationFunction func)
    {
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
}