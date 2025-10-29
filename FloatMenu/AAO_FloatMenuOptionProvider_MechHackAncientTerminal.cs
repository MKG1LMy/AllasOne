using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace AllasOne.FloatMenuOptions
{
    public class FloatMenuOptionProvider_AAO_MechHack : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool RequiresManipulation => true;

        protected override bool MechanoidCanDo => true;

        protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (MC == null) return null;

            if (!context.FirstSelectedPawn.IsColonyMech || context.FirstSelectedPawn?.GetOverseer() != MC) return null;

            //if (!ModsConfig.IdeologyActive || clickedThing.def != ThingDefOf.AncientEnemyTerminal)
            //{
            //    return null;
            //}
            if (!clickedThing.TryGetComp(out CompHackable comp) || comp.Props.onlyRemotelyHackable)
            {
                return null;
            }
            if (!comp.CanHackNow(context.FirstSelectedPawn).Accepted)
            {
                return null;
            }
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Hack".Translate(clickedThing.Label), delegate
            {
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmHackEnemyTerminal".Translate(ThingDefOf.AncientEnemyTerminal.label), delegate
                {
                    context.FirstSelectedPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Hack, clickedThing), JobTag.Misc);
                }));
            }), context.FirstSelectedPawn, new LocalTargetInfo(clickedThing));
        }
    }

}
