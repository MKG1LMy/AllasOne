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
    [HarmonyPatch(typeof(RitualOutcomeComp_ParticipantCount), "Counts")]
    public static class Patch_RitualOutcomeComp_ParticipantCount_Counts
    {
        public static bool Prefix(
            RitualOutcomeComp_ParticipantCount __instance,
            RitualRoleAssignments assignments,
            Pawn p,
            ref bool __result)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null) return true; // 运行原版

            try
            {

                if (assignments != null && assignments.Ritual == null && assignments.Required(p))
                {
                    __result = false;
                    return false;
                }

                RitualRole ritualRole = assignments?.RoleForPawn(p);
                if (ritualRole != null && !ritualRole.countsAsParticipant)
                {
                    __result = false;
                    return false;
                }
                __result = (p.RaceProps.Humanlike || (p.IsColonyMech && p?.GetOverseer() == MC));
                return false;  // 跳过原方法
            }
            catch (Exception ex)
            {
                Log.Error($"[AAO][Counts Patch] Exception: {ex}");
                return true; // 出错则回退到原版
            }
        }
    }
}
