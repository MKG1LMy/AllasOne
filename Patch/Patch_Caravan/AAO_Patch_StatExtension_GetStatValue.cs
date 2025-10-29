using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Caravan
{
    [HarmonyPatch(typeof(StatExtension), nameof(StatExtension.GetStatValue))]
    public static class GetStatValue_Patch
    {
        public static void Postfix(this Thing thing, StatDef stat, ref float __result, bool applyPostProcess = true, int cacheStaleAfterTicks = -1)
        {
            var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            var myConscious = mgr?.MechanoidConsciousness;

            Pawn pawn = thing as Pawn;
            if (pawn != null && pawn.IsColonyMechPlayerControlled && stat == StatDefOf.TradePriceImprovement && pawn.GetOverseer() == myConscious)
            {
                if (pawn.GetOverseer() != null)
                {
                    Thing overseer = pawn.GetOverseer();
                    __result = stat.Worker.GetValue(overseer, applyPostProcess, cacheStaleAfterTicks);
                }
            }
        }
    }
}
