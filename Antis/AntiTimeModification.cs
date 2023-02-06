using System.Threading;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class AntiTimeModification
{
    [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
    private static extern uint MM_GetTime();

    [DllImport("kernel32.dll")]
    static extern ulong GetTickCount64();

    public static bool IsTimeModified(AntiTimeModificationFunction function)
    {
        if (function.Equals(AntiTimeModificationFunction.GetTickCount))
        {
            long tickCount = Environment.TickCount;
            Thread.Sleep(500);
            long tickCount2 = Environment.TickCount;
            return (tickCount2 - tickCount) > 600L;
        }
        else if (function.Equals(AntiTimeModificationFunction.GetTickCount64))
        {
            ulong tickCount = GetTickCount64();
            Thread.Sleep(500);
            ulong tickCount2 = GetTickCount64();
            return (tickCount2 - tickCount) > 600L;
        }
        else if (function.Equals(AntiTimeModificationFunction.QueryPerformanceCounter))
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Thread.Sleep(500);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds > 600L;
        }
        else if (function.Equals(AntiTimeModificationFunction.timeGetTime))
        {
            long milliseconds1 = MM_GetTime();
            Thread.Sleep(500);
            long milliseconds2 = MM_GetTime();
            return (milliseconds2 - milliseconds1) > 600L;
        }

        return false;
    }
}

public enum AntiTimeModificationFunction
{
    QueryPerformanceCounter,
    GetTickCount,
    GetTickCount64,
    timeGetTime
}