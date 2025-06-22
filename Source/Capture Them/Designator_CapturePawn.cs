using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureThem;

public class Designator_CapturePawn : Designator
{
    public Designator_CapturePawn()
    {
        defaultLabel = "DesignatorCapturePawn".Translate();
        defaultDesc = "DesignatorCapturePawnDesc".Translate();
        icon = ContentFinder<Texture2D>.Get("CapturePawnGizmo");
        useMouseIcon = true;
        soundSucceeded = SoundDefOf.Designate_Haul;
        hotKey = KeyBindingDefOf.Misc1;
    }

    protected override DesignationDef Designation => CaptureThemDefOf.CaptureThemCapture;

    public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Orders;

    public override AcceptanceReport CanDesignateCell(IntVec3 loc)
    {
        if (!loc.InBounds(Map) || loc.Fogged(Map))
        {
            return false;
        }

        var firstPawn = loc.GetFirstPawn(Map);
        if (firstPawn == null || !firstPawn.RaceProps.Humanlike)
        {
            return "MessageMustDesignateDownedForeignPawn".Translate();
        }

        var result = CanDesignateThing(firstPawn);
        if (!result.Accepted)
        {
            return result;
        }

        return true;
    }

    public override AcceptanceReport CanDesignateThing(Thing t)
    {
        if (Map.designationManager.DesignationOn(t, Designation) != null)
        {
            return false;
        }

        return t is Pawn { Downed: true } pawn && pawn.Faction != Faction.OfPlayer && !pawn.InBed() &&
               !pawn.IsPrisonerOfColony && pawn.RaceProps.Humanlike;
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
        var thingList = c.GetThingList(Map);
        foreach (var thing in thingList)
        {
            if (thing is Pawn pawn)
            {
                DesignateThing(pawn);
            }
        }
    }

    public override void DesignateThing(Thing t)
    {
        var pawn = t as Pawn;
        if (pawn?.Faction != null && pawn.Faction != Faction.OfPlayer && !pawn.Faction.Hidden &&
            !pawn.Faction.HostileTo(Faction.OfPlayer) && !pawn.IsPrisonerOfColony && pawn.RaceProps.Humanlike)
        {
            Messages.Message("MessageCapturingWillAngerFaction".Translate(pawn.Named("PAWN")).AdjustedFor(pawn), pawn,
                MessageTypeDefOf.CautionInput, false);
        }

        Map.designationManager.RemoveAllDesignationsOn(t);
        Map.designationManager.AddDesignation(new Designation(t, Designation));
    }
}