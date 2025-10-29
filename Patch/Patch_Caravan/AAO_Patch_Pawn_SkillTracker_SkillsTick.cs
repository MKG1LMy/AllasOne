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
    [HarmonyPatch(typeof(Pawn_SkillTracker), "SkillsTickInterval")]
    public static class Patch_Pawn_SkillTracker_SkillsTickInterval
    {
        public static bool Prefix(Pawn_SkillTracker __instance)
        {
            var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            var myConscious = mgr?.MechanoidConsciousness;

            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn != null && pawn.GetOverseer() == myConscious)
            {
                // 如果满足条件，那么就取消原始方法的执行
                return false;
            }

            // 如果不满足条件，那么就继续执行原始方法
            return true;
        }
    }
}
