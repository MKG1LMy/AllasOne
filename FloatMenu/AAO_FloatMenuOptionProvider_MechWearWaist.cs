using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AllasOne.FloatMenu
{
    public class FloatMenuOptionProvider_MechWearWaist : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool MechanoidCanDo => true;

        protected override bool AppliesInt(FloatMenuContext context)
        {
            if (!context.FirstSelectedPawn.RaceProps.IsMechanoid)
            {
                return false;
            }

            Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null || !context.FirstSelectedPawn.IsColonyMech || context.FirstSelectedPawn?.GetOverseer() != MC)
            {
                return false;
            }
            return true;

        }

        protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        {
            Apparel apparel = clickedThing as Apparel;
            if (apparel == null || apparel.def.apparel.bodyPartGroups.Any((BodyPartGroupDef bpd) => bpd.defName != "Waist" ))
            {
                return null;
            }
            string key = "CannotWear";
            string key2 = "ForceWear";
            if (apparel.def.apparel.LastLayer.IsUtilityLayer)
            {
                key = "CannotEquipApparel";
                key2 = "ForceEquipApparel";
            }
            if (!context.FirstSelectedPawn.CanReach(apparel, PathEndMode.ClosestTouch, Danger.Deadly))
            {
                return new FloatMenuOption(key.Translate(apparel.Label, apparel) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
            }
            if (apparel.IsBurning())
            {
                return new FloatMenuOption(key.Translate(apparel.Label, apparel) + ": " + "Burning".Translate(), null);
            }
            if (context.FirstSelectedPawn.apparel.WouldReplaceLockedApparel(apparel))
            {
                return new FloatMenuOption(key.Translate(apparel.Label, apparel) + ": " + "WouldReplaceLockedApparel".Translate().CapitalizeFirst(), null);
            }
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(key2.Translate(apparel.LabelShort, apparel), delegate
            {
                Action action = delegate
                {
                    apparel.SetForbidden(value: false);
                    Job job = JobMaker.MakeJob(AllasOne.WorkandJob.AAO_JobDefOf.AAO_MechWearWaist, apparel);
                    context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                };
                Apparel apparelReplacedByNewApparel = ApparelUtility.GetApparelReplacedByNewApparel(context.FirstSelectedPawn, apparel);
                if (apparelReplacedByNewApparel == null || !ModsConfig.BiotechActive || !MechanitorUtility.TryConfirmBandwidthLossFromDroppingThing(context.FirstSelectedPawn, apparelReplacedByNewApparel, action))
                {
                    action();
                }
            }, MenuOptionPriority.High), context.FirstSelectedPawn, apparel);
        }
    }

}
