using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Ritual
{
    [HarmonyPatch(typeof(RitualRoleAssignments), nameof(RitualRoleAssignments.CanEverSpectate))]
    public static class Patch_RitualRoleAssignments_CanEverSpectate
    {
        static bool Prefix(RitualRoleAssignments __instance, Pawn pawn, ref bool __result)
        {

            // 取原类私有字段 ritual 与 ritualTarget
            var fRitual = AccessTools.Field(typeof(RitualRoleAssignments), "ritual");
            var fTarget = AccessTools.Field(typeof(RitualRoleAssignments), "ritualTarget");
            var ritual = (Precept_Ritual)fRitual.GetValue(__instance);
            var ritualTarget = (TargetInfo)fTarget.GetValue(__instance);

            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null) return true;
            else if (pawn.GetOverseer() == MC && !pawn.Dead)
            {
                __result = true;  // 允许观礼
                //Log.Message("AAO Patch CanEverSpectate: allow " + pawn.Name.ToStringShort);
                return false;    // 跳过原版
            }
            Log.Message("AAO Patch CanEverSpectate: disbale " + pawn.Name.ToStringShort);
            return true; // 跳过原方法
        }
    }


}
