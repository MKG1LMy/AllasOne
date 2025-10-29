using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_VoidMonolith
{
    [HarmonyPatch(typeof(Building_VoidMonolith), "Activate")]
    public static class Building_VoidMonolith_Activate
    {
        public static void Prefix(ref Pawn pawn)
        {
            var myConscious = AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;

            if (pawn != null && pawn.RaceProps.IsMechanoid && pawn.GetOverseer() == myConscious)
            {
                pawn = pawn.GetOverseer();
            }
        }
    }
}
