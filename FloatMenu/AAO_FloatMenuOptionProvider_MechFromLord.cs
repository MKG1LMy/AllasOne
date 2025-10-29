using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace AllasOne.FloatMenuOptions
{
    public class FloatMenuOptionProvider_AAO_MechFromLord : FloatMenuOptionProvider
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

        public override IEnumerable<FloatMenuOption> GetOptionsFor(Pawn clickedPawn, FloatMenuContext context)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (MC == null) return null;

            if (!context.FirstSelectedPawn.IsColonyMech || context.FirstSelectedPawn?.GetOverseer() != MC) return null;

            Lord lord = clickedPawn.GetLord();
            if (lord == null)
            {
                return Enumerable.Empty<FloatMenuOption>();
            }
            return lord.CurLordToil.ExtraFloatMenuOptions(clickedPawn, context.FirstSelectedPawn);
        }
    }

}
