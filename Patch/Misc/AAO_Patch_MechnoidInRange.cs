using AllasOne.WorldandGame;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch
{
    // 目标：若 mech 的 overseer 是我们的“机械族意识”，则无条件允许（返回 true）
    [HarmonyPatch(typeof(MechanitorUtility), nameof(MechanitorUtility.InMechanitorCommandRange))]
    public static class Patch_InMechanitorCommandRange_AllowForConsciousness
    {
        // 原签名：public static bool InMechanitorCommandRange(Pawn mech, LocalTargetInfo target)
        public static void Postfix(Pawn mech, LocalTargetInfo target, ref bool __result)
        {
            if (__result || mech == null) return;

            // 拿到“我的那一个意识体”
            var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            var myConscious = mgr?.MechanoidConsciousness;
            if (myConscious == null) return;

            // overseer 命中 → 放行
            var overseer = mech.GetOverseer();
            if (overseer == myConscious)
            {
                __result = true;
            }
        }
    }
}
