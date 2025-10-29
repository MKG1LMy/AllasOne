using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Alert
{
    [HarmonyPatch(typeof(Alert_NeedMeditationSpot), "get_Targets")]
    public static class Patch_Alert_NeedMeditationSpot_Targets_RoleFiltered
    {
        public static bool Prefix(Alert_NeedMeditationSpot __instance, ref List<GlobalTargetInfo> __result)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            if (MC == null) return true; // 放行原版

            try
            {
                // 取私有字段
                var fTargets = AccessTools.Field(typeof(Alert_NeedMeditationSpot), "targets");
                var fNames = AccessTools.Field(typeof(Alert_NeedMeditationSpot), "pawnNames");
                var fCache = AccessTools.Field(typeof(Alert_NeedMeditationSpot), "CachedSpots"); // static

                var targets = new List<GlobalTargetInfo>();
                var names = new List<string>();
                var cache = (Dictionary<Pawn, Building>)(fCache.GetValue(null) ?? new Dictionary<Pawn, Building>());

                foreach (Pawn pawn in PawnsFinder.HomeMaps_FreeColonistsSpawned)
                {
                    // ← 仅新增这一条：不符合你定义的“角色可用”就跳过
                    if (!IsRoleEligible(pawn)) continue;

                    bool scheduledMeditate = false;
                    for (int i = 0; i < pawn.timetable.times.Count; i++)
                        if (pawn.timetable.times[i] == TimeAssignmentDefOf.Meditate) { scheduledMeditate = true; break; }

                    if ((!pawn.HasPsylink && !scheduledMeditate) || !pawn.psychicEntropy.IsPsychicallySensitive)
                        continue;

                    if (cache.TryGetValue(pawn, out var cachedBld))
                    {
                        if (MeditationUtility.IsValidMeditationBuildingForPawn(cachedBld, pawn))
                            continue;
                        cache.Remove(pawn);
                    }

                    var spot = MeditationUtility
                        .AllMeditationSpotCandidates(pawn, allowFallbackSpots: false)
                        .FirstOrFallback(LocalTargetInfo.Invalid);

                    if (!spot.IsValid)
                    {
                        targets.Add(pawn);
                        names.Add(pawn.NameShortColored.Resolve());
                    }
                    else if (spot.Thing is Building b2)
                    {
                        cache[pawn] = b2;
                    }
                }

                // 回填实例/静态字段，保持下游说明文本一致
                fTargets.SetValue(__instance, targets);
                fNames.SetValue(__instance, names);
                fCache.SetValue(null, cache);

                __result = targets;
                return false; // 跳过原版 getter
            }
            catch (Exception ex)
            {
                Log.Error($"[AAO][MeditationTargetsRoleFilter] {ex}");
                return true; // 出错放行原版
            }
        }

        // 你定义的“是否适用某个角色”判定；按需改写
        static bool IsRoleEligible(Pawn p)
        {
            if (p.RaceProps.IsMechanoid)
            {
                return false; // 机械人不需要
            }

            return true; // 默认放行；请替换为你的规则
        }
    }
}
