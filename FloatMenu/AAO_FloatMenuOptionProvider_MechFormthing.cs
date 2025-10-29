using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.FloatMenuOptions
{
    public class FloatMenuOptionProvider_FromThing : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => true;

        protected override bool CanSelfTarget => true;

        protected override bool MechanoidCanDo => true;

        public override IEnumerable<FloatMenuOption> GetOptionsFor(Thing clickedThing, FloatMenuContext context)
        {
            Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;

            if (context.FirstSelectedPawn.GetOverseer() == MC)
            {
                if (context.IsMultiselect)
                {
                    foreach (FloatMenuOption multiSelectFloatMenuOption in clickedThing.GetMultiSelectFloatMenuOptions(context.ValidSelectedPawns))
                    {
                        yield return multiSelectFloatMenuOption;
                    }
                    yield break;
                }
                foreach (FloatMenuOption floatMenuOption in clickedThing.GetFloatMenuOptions(context.FirstSelectedPawn))
                {
                    yield return floatMenuOption;
                }
            }            
        }
    }

}
