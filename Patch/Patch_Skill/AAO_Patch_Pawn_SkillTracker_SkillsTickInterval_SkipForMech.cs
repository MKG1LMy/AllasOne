using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Skill
{
    [HarmonyPatch(typeof(Pawn_SkillTracker), "SkillsTickInterval")]
    static class Patch_Pawn_SkillTracker_SkillsTickInterval_SkipForMech
    {
        public static bool Prefix(object __instance, int delta)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;

            try
            {
                FieldInfo fPawn = AccessTools.Field(typeof(Pawn_SkillTracker), "pawn");
                if (fPawn == null)
                {
                    return true;
                }

                var pawnObj = fPawn.GetValue(__instance) as Pawn;
                if (pawnObj == null)
                {
                    return true;
                }

                if (pawnObj?.GetOverseer() == MC)
                {
                    return false; // 跳过原方法
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Patch] SkillsTickInterval Prefix failed: {ex}");
            }

            return true;
        }
    }
}
