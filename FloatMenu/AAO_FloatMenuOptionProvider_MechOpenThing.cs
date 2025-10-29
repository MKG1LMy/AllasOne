using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AllasOne.FloatMenuOptions
{
    public class FloatMenuOptionProvider_AAO_MechOpenThing : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool MechanoidCanDo => true;

        protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (MC == null) return null;

            if (!context.FirstSelectedPawn.IsColonyMech || context.FirstSelectedPawn?.GetOverseer() != MC) return null;

            var openable = clickedThing as IOpenable;              
            if (openable == null || !openable.CanOpen)               
            {                   
                return null;               
            }
            
            if (!context.FirstSelectedPawn.CanReach(clickedThing, PathEndMode.OnCell, Danger.Deadly))
            {
                return new FloatMenuOption("CannotOpen".Translate(clickedThing) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
            }
            if (!context.FirstSelectedPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                return new FloatMenuOption("CannotOpen".Translate(clickedThing) + ": " + "Incapable".Translate().CapitalizeFirst(), null);
            }
            if (!context.FirstSelectedPawn.Drafted && clickedThing.Map.designationManager.DesignationOn(clickedThing, DesignationDefOf.Open) != null)
            {
                return null;
            }
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Open".Translate(clickedThing), delegate
            {
                Job job = JobMaker.MakeJob(JobDefOf.Open, clickedThing);
                job.ignoreDesignations = true;
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }, MenuOptionPriority.High), context.FirstSelectedPawn, clickedThing);
        }
    }
}
