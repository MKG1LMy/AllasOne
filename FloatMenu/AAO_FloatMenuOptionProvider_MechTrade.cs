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
    internal class FloatMenuOptionProvider_AAO_MechTrade : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;
        protected override bool Undrafted => true;
        protected override bool Multiselect => false;
        protected override bool RequiresManipulation => false;
        protected override bool MechanoidCanDo => true;
        protected override bool CanSelfTarget => true;

        public override IEnumerable<FloatMenuOption> GetOptionsFor(Pawn clickedPawn, FloatMenuContext context)
        {
            Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            Pawn Mech = context.FirstSelectedPawn;
            if (Mech.IsColonyMech && Mech.GetOverseer() == MC)
            {
                if (clickedPawn == null || !((ITrader)clickedPawn).CanTradeNow)
                {
                    yield break;
                }
                if (!context.FirstSelectedPawn.CanReach(clickedPawn, PathEndMode.OnCell, Danger.Deadly))
                {
                    yield return new FloatMenuOption("CannotTrade".Translate() + ": " + "NoPath".Translate().CapitalizeFirst(), null);
                    yield break;
                }
                if (context.FirstSelectedPawn.skills.GetSkill(SkillDefOf.Social).TotallyDisabled)
                {
                    yield return new FloatMenuOption("CannotPrioritizeWorkTypeDisabled".Translate(SkillDefOf.Social.LabelCap), null);
                    yield break;
                }
                if (clickedPawn.mindState.traderDismissed)
                {
                    yield return new FloatMenuOption("TraderDismissed".Translate(), null);
                    yield break;
                }
                if (!context.FirstSelectedPawn.CanTradeWith(clickedPawn.Faction, clickedPawn.TraderKind).Accepted)
                {
                    yield return new FloatMenuOption("CannotTrade".Translate() + ": " + "MissingTitleAbility".Translate().CapitalizeFirst(), null);
                }
                else
                {
                    Action action = delegate
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.TradeWithPawn, clickedPawn);
                        job.playerForced = true;
                        context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.InteractingWithTraders, KnowledgeAmount.Total);
                    };
                    string text = "";
                    if (clickedPawn.Faction != null)
                    {
                        text = " (" + clickedPawn.Faction.Name + ")";
                    }
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("TradeWith".Translate(clickedPawn.LabelShort + ", " + clickedPawn.TraderKind.label) + text, action, MenuOptionPriority.InitiateSocial, null, clickedPawn), context.FirstSelectedPawn, clickedPawn);
                }
                if (clickedPawn.GetLord().LordJob is LordJob_TradeWithColony && !clickedPawn.mindState.traderDismissed)
                {
                    Action action2 = delegate
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.DismissTrader, clickedPawn);
                        job.playerForced = true;
                        context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    };
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("DismissTrader".Translate(), action2, MenuOptionPriority.InitiateSocial, null, clickedPawn), context.FirstSelectedPawn, clickedPawn);
                }

            }




            else
            {
                yield break;
            }
        }

    }
}
