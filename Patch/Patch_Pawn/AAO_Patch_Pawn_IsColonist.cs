using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Pawn
{
    [HarmonyPatch(typeof(Pawn), "get_IsColonist")]
    static class AAO_Patch_Pawn_IsColonist_Prefix
    {
        // Prefix 方法：我们修改返回值
        public static bool Prefix(Pawn __instance, ref bool __result)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null) return true; // 放行原版逻辑

            // 检查是否是机械族并且有 overseer
            if (__instance.RaceProps.IsMechanoid && __instance.GetOverseer() != null && __instance.IsColonyMech && __instance.GetOverseer() == MC)
            {
                __result = true;
                return false; // 返回 false，表示不再执行原始的 get_IsColonist 方法
            }

            // 其他正常情况，让原方法逻辑继续执行
            return true; // 继续执行原方法
        }
    }
}
