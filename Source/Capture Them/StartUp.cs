using System.Reflection;
using HarmonyLib;
using Verse;

namespace CaptureThem;

[StaticConstructorOnStartup]
public static class StartUp
{
    static StartUp()
    {
        new Harmony("CaptureThem.patch").PatchAll(Assembly.GetExecutingAssembly());
    }
}