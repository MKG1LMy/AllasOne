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
    [HarmonyPatch(typeof(RitualRoleColonist), nameof(RitualRoleColonist.AppliesToPawn))]
    public static class Patch_RitualRoleColonist_AppliesToPawn
    {

        static bool Prefix(
            RitualRoleColonist __instance,
            Pawn p,
            ref string reason,                 // Harmony 将 out 视作 ref
            TargetInfo selectedTarget,
            LordJob_Ritual ritual,
            RitualRoleAssignments assignments,
            Precept_Ritual precept,
            bool skipReason,
            ref bool __result)
        
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null) return true;


            else if (p.GetOverseer() == MC && !p.Dead)
            {
                // 你自己的逻辑：示例为放宽部分限制
                reason = null;
                __result = true;     // 允许担任该角色
                return false;        // 跳过原版                
            }


            return true; // 其他情况运行原版
        }


        
    }
}
