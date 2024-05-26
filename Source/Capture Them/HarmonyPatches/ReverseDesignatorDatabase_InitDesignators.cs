using CaptureThem;
using HarmonyLib;
using Verse;

namespace Capture_Them.HarmonyPatches;

[HarmonyPatch(typeof(ReverseDesignatorDatabase), "InitDesignators")]
public class ReverseDesignatorDatabase_InitDesignators
{
    public static void Postfix(ref ReverseDesignatorDatabase __instance)
    {
        __instance.AllDesignators.Add(new Designator_CapturePawn());
    }
}