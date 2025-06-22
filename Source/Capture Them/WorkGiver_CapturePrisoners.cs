using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureThem;

public class WorkGiver_CapturePrisoners : WorkGiver_RescueDowned
{
    private static JobDef Job => JobDefOf.Capture;

    private static DesignationDef Designation => CaptureThemDefOf.CaptureThemCapture;

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return !pawn.Map.designationManager.AnySpawnedDesignationOfDef(Designation);
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        foreach (var designation in pawn.Map.designationManager.SpawnedDesignationsOfDef(Designation))
        {
            yield return designation.target.Thing;
        }
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Pawn { Downed: true } pawn2 || pawn2.Faction == pawn.Faction ||
            t.Map.designationManager.DesignationOn(t, Designation) == null)
        {
            return false;
        }

        if (pawn2.InBed() || !pawn.CanReserve(pawn2, 1, -1, null, forced) || dangerIsNear(pawn, pawn2, 40f))
        {
            return false;
        }

        var buildingBed = RestUtility.FindBedFor(pawn2, pawn, false, false, GuestStatus.Prisoner) ??
                          RestUtility.FindBedFor(pawn2, pawn, false, true, GuestStatus.Prisoner);

        if (buildingBed != null)
        {
            return pawn.CanReserve(buildingBed, 1, -1, null, forced);
        }

        Messages.Message("CannotCapture".Translate() + ": " + "NoPrisonerBed".Translate(), pawn2,
            MessageTypeDefOf.RejectInput, false);
        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var pawn2 = t as Pawn;
        var t2 = RestUtility.FindBedFor(pawn2, pawn, false, false, GuestStatus.Prisoner);
        var job = JobMaker.MakeJob(Job, pawn2, t2);
        job.count = 1;
        PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Capturing, KnowledgeAmount.Total);
        return job;
    }

    private static bool dangerIsNear(Pawn pawn, Pawn p, float radius)
    {
        if (!p.Spawned)
        {
            return false;
        }

        var fogged = p.Position.Fogged(p.Map);
        var potentialTargetsFor = p.Map.attackTargetsCache.GetPotentialTargetsFor(pawn);
        foreach (var attackTarget in potentialTargetsFor)
        {
            if (!attackTarget.ThreatDisabled(pawn) &&
                (fogged || !attackTarget.Thing.Position.Fogged(attackTarget.Thing.Map)) &&
                p.Position.InHorDistOf(((Thing)attackTarget).Position, radius))
            {
                return true;
            }
        }

        return false;
    }
}