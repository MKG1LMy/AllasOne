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
    public class AAO_FloatMenuOptionProvider_MechReturnSlaveOrPrisonToBed: FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool RequiresManipulation => true;

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

        protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
        {
            if (!clickedPawn.IsSlaveOfColony && !clickedPawn.IsPrisonerOfColony)
            {
                return null;
            }
            if (clickedPawn.InBed())
            {
                return null;
            }
            FloatMenuOption floatMenuOption = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("ReturnToBed".Translate(), delegate
            {

                Building_Bed building_Bed;
                if (clickedPawn.IsPrisonerOfColony)
                {
                    building_Bed = RestUtility.FindBedFor(clickedPawn, context.FirstSelectedPawn, checkSocialProperness: false, ignoreOtherReservations: false, GuestStatus.Prisoner);
                    if (building_Bed == null)
                    {
                        building_Bed = RestUtility.FindBedFor(clickedPawn, context.FirstSelectedPawn, checkSocialProperness: false, ignoreOtherReservations: true, GuestStatus.Prisoner);
                    }
                }
                else
                {
                    building_Bed = RestUtility.FindBedFor(clickedPawn, context.FirstSelectedPawn, checkSocialProperness: false, ignoreOtherReservations: false, GuestStatus.Slave);
                    if (building_Bed == null)
                    {
                        building_Bed = RestUtility.FindBedFor(clickedPawn, context.FirstSelectedPawn, checkSocialProperness: false, ignoreOtherReservations: true, GuestStatus.Slave);
                    }
                }               
                if (building_Bed == null)
                {
                    Messages.Message("Cannot".Translate() + ": " + "NoBed".Translate(), clickedPawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                else
                {
                    Job job = JobMaker.MakeJob(JobDefOf.TakeToBedToOperate, clickedPawn, building_Bed);
                    job.count = 1;
                    context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Capturing, KnowledgeAmount.Total);
                }
            }, MenuOptionPriority.RescueOrCapture, null, clickedPawn), context.FirstSelectedPawn, clickedPawn);
            string cannot = string.Format("{0}: {1}", "Cannot".Translate(), "NoBed".Translate());
            FloatMenuUtility.ValidateTakeToBedOption(context.FirstSelectedPawn, clickedPawn, floatMenuOption, cannot, GuestStatus.Prisoner);
            return floatMenuOption;
        }

        
    } 
}
