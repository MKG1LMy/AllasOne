using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using UnityEngine;

namespace AllasOne.FloatMenuOptions
{
    public class FloatMenuOptionProvider_AAO_MechCaptureEntity : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool RequiresManipulation => true;

        protected override bool MechanoidCanDo => true;

        protected override bool AppliesInt(FloatMenuContext context)
        {
            return ModsConfig.AnomalyActive;
        }

        public override IEnumerable<FloatMenuOption> GetOptionsFor(Thing clickedThing, FloatMenuContext context)
        {
            Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (MC == null) yield break;

            if (context.FirstSelectedPawn.GetOverseer() == MC)
            {
                if (!clickedThing.TryGetComp(out CompHoldingPlatformTarget holdComp) || !holdComp.CanBeCaptured || !holdComp.StudiedAtHoldingPlatform)
                {
                    yield break;
                }
                if (!context.FirstSelectedPawn.CanReserveAndReach(clickedThing, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true))
                {
                    yield return new FloatMenuOption("CannotGenericWorkCustom".Translate("CaptureLower".Translate(clickedThing)) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
                    yield break;
                }
                IEnumerable<Building_HoldingPlatform> buildings = from x in context.FirstSelectedPawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building_HoldingPlatform>()
                                                                  where !x.Occupied && context.FirstSelectedPawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.Deadly)
                                                                  select x;
                Thing building = GenClosest.ClosestThing_Global_Reachable(context.FirstSelectedPawn.Position, context.FirstSelectedPawn.Map, buildings, PathEndMode.ClosestTouch, TraverseParms.For(context.FirstSelectedPawn, Danger.Some), 9999f, null, delegate (Thing t)
                {
                    CompEntityHolder compEntityHolder = t.TryGetComp<CompEntityHolder>();
                    return (compEntityHolder != null && compEntityHolder.ContainmentStrength >= clickedThing.GetStatValue(StatDefOf.MinimumContainmentStrength)) ? (compEntityHolder.ContainmentStrength / Mathf.Max(clickedThing.PositionHeld.DistanceTo(t.Position), 1f)) : 0f;
                });
                if (building == null)
                {
                    yield return new FloatMenuOption("CannotGenericWorkCustom".Translate("CaptureLower".Translate(clickedThing)) + ": " + "NoHoldingPlatformsAvailable".Translate().CapitalizeFirst(), null);
                    yield break;
                }
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Capture".Translate(clickedThing.Label, clickedThing), delegate
                {
                    if (!ContainmentUtility.SafeContainerExistsFor(clickedThing))
                    {
                        Messages.Message("MessageNoRoomWithMinimumContainmentStrength".Translate(clickedThing.Label), MessageTypeDefOf.ThreatSmall);
                    }
                    holdComp.targetHolder = building;
                    Job job = JobMaker.MakeJob(JobDefOf.CarryToEntityHolder, building, clickedThing);
                    job.count = 1;
                    context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                }), context.FirstSelectedPawn, clickedThing);
                if (buildings.Count() > 1)
                {
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Capture".Translate(clickedThing.Label, clickedThing) + " (" + "ChooseEntityHolder".Translate() + "...)", delegate
                    {
                        StudyUtility.TargetHoldingPlatformForEntity(context.FirstSelectedPawn, clickedThing);
                    }), context.FirstSelectedPawn, clickedThing);
                }
            }

            
        }
    }
}
