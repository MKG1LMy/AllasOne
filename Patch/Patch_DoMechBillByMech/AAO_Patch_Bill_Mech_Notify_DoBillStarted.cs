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
    [HarmonyPatch(typeof(Bill_Mech), nameof(Bill_Mech.Notify_DoBillStarted))]
    public static class Bill_Mech_Notify_DoBillStarted_Patch
    {
        // 缓存对私有字段 boundPawn 的引用
        private static readonly AccessTools.FieldRef<Bill_Mech, Pawn> BoundPawnRef =
            AccessTools.FieldRefAccess<Bill_Mech, Pawn>("boundPawn");

        public static void Postfix(Bill_Mech __instance, Pawn billDoer)
        {
            var myConscious = AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (myConscious == null) return;

            if (billDoer.RaceProps.IsMechanoid && billDoer.GetOverseer() == myConscious)
            {
                // 相当于 __instance.boundPawn = overseer;
                BoundPawnRef(__instance) = billDoer.GetOverseer();
            }
        }
    }
}
