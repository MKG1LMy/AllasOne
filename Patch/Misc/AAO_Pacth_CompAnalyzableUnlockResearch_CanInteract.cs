using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

namespace AllasOne.Patch.Misc
{
    [HarmonyPatch(typeof(CompAnalyzableUnlockResearch))]
    [HarmonyPatch(nameof(CompAnalyzableUnlockResearch.CanInteract))]
    public static class Patch_CompAnalyzableUnlockResearch_CanInteract
    {
        public static void Postfix(CompAnalyzableUnlockResearch __instance, Pawn activateBy, bool checkOptionalItems, ref AcceptanceReport __result)
        {
            if (__result.Accepted) return; 

            // 推荐用 Reason 字段做比较
            if (!__result.Accepted && __result.Reason == "RequiresMechanitor".Translate())
            {                
                var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
                if (MC == null || activateBy == null) return;

                if (activateBy?.GetOverseer() == MC && __instance.Props.requiresMechanitor)
                {
                    __result = true;
                }
            }


        }
    }
}
