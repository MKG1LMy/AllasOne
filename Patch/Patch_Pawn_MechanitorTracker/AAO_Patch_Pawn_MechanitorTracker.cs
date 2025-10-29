using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using AllasOne.WorldandGame;

namespace AllasOne.Patch.Patch_Pawn_MechanitorTracker
{
    [HarmonyPatch(typeof(Pawn_MechanitorTracker))]
    [HarmonyPatch(nameof(Pawn_MechanitorTracker.Notify_Downed))]
    public static class Patch_Pawn_MechanitorTracker_Notify_Downed_Prefix
    {

        static bool Prefix(Pawn_MechanitorTracker __instance)
        {
            // 通过反射安全取出私有字段 pawn（字段名在不同版本可能不同，常见名为 "pawn"）
            var pawnField = AccessTools.Field(typeof(Pawn_MechanitorTracker), "pawn");
            Pawn pawn = pawnField != null ? pawnField.GetValue(__instance) as Pawn : null;

            var MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (MC == null || pawn == null || pawn != MC)
            {
                return true; // 继续执行原方法
            }

            return false; // 跳过原方法

        }
    }

    [HarmonyPatch(typeof(Pawn_MechanitorTracker))]
    [HarmonyPatch(nameof(Pawn_MechanitorTracker.Notify_DeSpawned))]
    public static class Patch_Pawn_MechanitorTracker_Notify_DeSpawned_Prefix
    {

        static bool Prefix(Pawn_MechanitorTracker __instance)
        {
            // 通过反射安全取出私有字段 pawn（字段名在不同版本可能不同，常见名为 "pawn"）
            var pawnField = AccessTools.Field(typeof(Pawn_MechanitorTracker), "pawn");
            Pawn pawn = pawnField != null ? pawnField.GetValue(__instance) as Pawn : null;

            var MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (MC == null || pawn == null || pawn != MC)
            {
                return true; // 继续执行原方法
            }

            return false; // 跳过原方法

        }
    }
}
