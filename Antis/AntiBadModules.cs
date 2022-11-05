using Reloaded.Memory.Sigscan;
using System.Diagnostics;

public class AntiBadModules
{
    private static string[] blockedPatterns = new string[]
    {
        "56 45 48 44 65 62 75 67 20 69 6E 69 74" // Cheat Engine, VEH Debugger
        // TODO: Cheat Engine Speed Hack, Sandbox things, other debuggers, Discord overlay, more.
        // TODO: Find another method to block the VEHDebugger. Check from original source:
        // https://github.com/cheat-engine/cheat-engine/tree/master/Cheat%20Engine/VEHDebug
        // https://github.com/cheat-engine/cheat-engine/blob/master/Cheat%20Engine/VEHDebugger.pas
        // https://forum.cheatengine.org/viewtopic.php?p=5772627&sid=352e465bee4a7f7bf99169e24a27da32
        // Lua Injectors (same for processes, luaL_newstate, lua_dump, ...).
    };

    private static string[] blockedModuleNames = new string[]
    {
        "speedhack", // Cheat Engine Speed Hack
        "vehdebug" // Cheat Engine VEH Debugger
    };

    public static bool IsBadModuleLoaded()
    {
        foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
        {
            try
            {
                if (module.ModuleName != Process.GetCurrentProcess().MainModule.ModuleName)
                {
                    string filteredModuleName = Utils.FilterString(module.ModuleName);

                    foreach (string moduleName in blockedModuleNames)
                    {
                        string newModuleName = Utils.FilterString(moduleName);

                        if (newModuleName.Contains(filteredModuleName))
                        {
                            return true;
                        }
                    }

                    Scanner scanner = new Scanner(Process.GetCurrentProcess(), module);

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
            catch
            {

            }
        }

        return false;
    }
}