using AllasOne.WorldandGame;
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
    public class AAO_FloatMenuOptionProvider_MechMechanitor : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool RequiresManipulation => true;

        protected override bool MechanoidCanDo => true;

        protected override bool AppliesInt(FloatMenuContext context)
        {
            Pawn MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null || !context.FirstSelectedPawn.RaceProps.IsMechanoid || !context.FirstSelectedPawn.IsColonyMech || context.FirstSelectedPawn?.GetOverseer() != MC)
            {
                return false;
            }
            return true;
        }

        public override IEnumerable<FloatMenuOption> GetOptionsFor(Pawn clickedPawn, FloatMenuContext context)
        {
            if (!clickedPawn.IsColonyMech)
            {
                yield break;
            }
            if (clickedPawn.GetOverseer() != context.FirstSelectedPawn.GetOverseer())
            {
                yield return new FloatMenuOption("CannotDisassembleMech".Translate(clickedPawn.LabelCap) + ": " + "MustBeOverseer".Translate().CapitalizeFirst(), null);
            }
            else
            {
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("DisconnectMech".Translate(clickedPawn.LabelShort), delegate
                {
                    MechanitorUtility.ForceDisconnectMechFromOverseer(clickedPawn);
                }, MenuOptionPriority.Low, null, null, 0f, null, null, playSelectionSound: true, -10), context.FirstSelectedPawn, new LocalTargetInfo(clickedPawn));
                if (!clickedPawn.IsFighting())
                {
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("DisassembleMech".Translate(clickedPawn.LabelCap), delegate
                    {
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDisassemblingMech".Translate(clickedPawn.LabelCap) + ":\n" + (from x in MechanitorUtility.IngredientsFromDisassembly(clickedPawn.def)
                                                                                                                                                        select x.Summary).ToLineList("  - "), delegate
                                                                                                                                                        {
                                                                                                                                                            context.FirstSelectedPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.DisassembleMech, clickedPawn), JobTag.Misc);
                                                                                                                                                        }, destructive: true));
                    }, MenuOptionPriority.Low, null, null, 0f, null, null, playSelectionSound: true, -20), context.FirstSelectedPawn, new LocalTargetInfo(clickedPawn));
                }
            }

        }

    }
}
