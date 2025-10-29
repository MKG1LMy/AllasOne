using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace AllasOne.Patch.Patch_Ritual
{
    [HarmonyPatch]
    public static class Patch_RitualRoleAssignments_PawnNotAssignableReason
    {
        // 精确绑定含 out bool 的重载
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                typeof(RitualRoleAssignments),
                nameof(RitualRoleAssignments.PawnNotAssignableReason),
                new Type[] {
                    typeof(Pawn),
                    typeof(RitualRole),
                    typeof(Precept_Ritual),
                    typeof(RitualRoleAssignments),
                    typeof(TargetInfo),
                    typeof(bool).MakeByRefType() // out bool stillAddToPawnList
                });
        }

        // Postfix：把“必须是人形(观众)”改为 null
        static void Postfix(
            Pawn p,
            RitualRole role,
            Precept_Ritual ritual,
            RitualRoleAssignments assignments,
            TargetInfo ritualTarget,
            ref bool stillAddToPawnList,
            ref string __result)
        {
            if (string.IsNullOrEmpty(__result)) return;

            string spectators = "Spectators".Translate().ToString();
            string humanlikeMsg = "MessageRitualRoleMustBeHumanlike".Translate(spectators).ToString();

            if (string.Equals(__result, humanlikeMsg, StringComparison.Ordinal))
            __result = null;
            //Log.Message("AAO Patch PawnNotAssignableReason: change " + humanlikeMsg + " to null for " + p.Label);
        }
    }
}



