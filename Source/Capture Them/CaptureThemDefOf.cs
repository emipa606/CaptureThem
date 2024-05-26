using RimWorld;
using Verse;

namespace CaptureThem;

[DefOf]
public static class CaptureThemDefOf
{
    public static DesignationDef CaptureThemCapture;

    static CaptureThemDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(CaptureThemDefOf));
    }
}