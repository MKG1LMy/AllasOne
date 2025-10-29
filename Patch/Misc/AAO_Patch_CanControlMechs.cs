using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch
{
    // 目标：若 pawn 拥有 AAO_Hediff_MechanoidConsciousnessNode，则 CanControlMechs 直接返回 true
    [HarmonyPatch(typeof(Pawn_MechanitorTracker))]
    [HarmonyPatch("get_CanControlMechs")]
    public static class Patch_Pawn_MechanitorTracker_CanControlMechs
    {
        public static bool Prefix(object __instance, ref AcceptanceReport __result)
        {
            // 取到私有字段 pawn
            var fPawn = AccessTools.Field(typeof(Pawn_MechanitorTracker), "pawn");
            var pawn = fPawn?.GetValue(__instance) as Pawn;
            if (pawn == null) return true; // 没拿到就走原逻辑

            // 取 HediffDef（找不到就不拦截）
            var hdef = DefDatabase<HediffDef>.GetNamedSilentFail("AAO_Hediff_MechanoidConsciousnessNode");
            if (hdef == null) return true;

            // 命中则直接放行：可控
            if (pawn.health?.hediffSet?.HasHediff(hdef) == true)
            {
                __result = true;   // AcceptanceReport 有隐式从 bool 转换
                return false;      // 跳过原方法
            }

            return true; // 不满足条件→走原逻辑
        }
    }
}
