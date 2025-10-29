using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_DoMechBillByMech
{
    [HarmonyPatch(typeof(WorkGiver_RepairMech), nameof(WorkGiver_RepairMech.ShouldSkip))]
    public static class WorkGiver_RepairMech_ShouldSkip_Postfix
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            var myConscious = AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (myConscious == null) return;

            // 只有“我方机仆 + 其overseer就是myConscious”时，强制不跳过
            if (pawn.IsColonyMech && pawn.GetOverseer() == myConscious)
            {
                if (__result) __result = false; // 原本要跳过 → 改为不跳过
            }
        }
    }
}
