using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;


namespace AllasOne.Patch.Patch_Ritual
{
    [HarmonyPatch(typeof(Precept_GravshipLaunch), nameof(Precept_GravshipLaunch.GetRitualBeginWindow))]
    public static class Patch_Precept_GravshipLaunch_GetRitualBeginWindow
    {
        // 条件满足 -> 运行原版 (return true)
        // 条件不满足 -> 运行自定义逻辑 (设置 __result, return false)
        static bool Prefix(Precept_GravshipLaunch __instance, ref Window __result, TargetInfo targetInfo, RitualObligation obligation, Action onConfirm, Pawn organizer, Dictionary<string, Pawn> forcedForRole, Pawn selectedPawn)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null) return true;

            else 
            {
                string text = __instance.behavior.CanStartRitualNow(targetInfo, __instance, selectedPawn, forcedForRole);
                if (!string.IsNullOrEmpty(text))
                {
                    Messages.Message(text, targetInfo, MessageTypeDefOf.RejectInput, historical: false);
                }
                __result = new Dialog_BeginGravshipLaunch(__instance.Label.CapitalizeFirst(), __instance, targetInfo, targetInfo.Map, delegate (RitualRoleAssignments assignments)
                {
                    __instance.behavior.TryExecuteOn(targetInfo, organizer, __instance, obligation, assignments, playerForced: true);
                    onConfirm?.Invoke();
                    return true;
                }, organizer, obligation, delegate (Pawn pawn, bool voluntary, bool allowOtherIdeos)
                {
                    if (pawn.GetLord() != null )
                    {
                        return false;
                    }
                    if (pawn.RaceProps.Animal && !__instance.behavior.def.roles.Any((RitualRole r) => r.AppliesToPawn(pawn, out var _, targetInfo, null, null, null, skipReason: true)))
                    {
                        return false;
                    }
                    if (pawn.IsSubhuman)
                    {
                        return false;
                    }
                    return !__instance.ritualOnlyForIdeoMembers || (pawn?.GetOverseer()==MC) || __instance.def.allowSpectatorsFromOtherIdeos || pawn.Ideo == __instance.ideo || !voluntary || allowOtherIdeos || pawn.IsPrisonerOfColony || pawn.RaceProps.Animal || (!forcedForRole.NullOrEmpty() && forcedForRole.ContainsValue(pawn));
                }, "Begin".Translate(), (organizer != null) ? new List<Pawn> { organizer } : null, forcedForRole, null, null, selectedPawn);
                
                Log.Message("AAO Patch Replace GetRitualBeginWindow");
                return false;
            }
        }
    }


}
