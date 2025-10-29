using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace AllasOne.Patch.Patch_Ritual
{
    [HarmonyPatch(typeof(RitualBehaviorWorker_GravshipLaunch), nameof(RitualBehaviorWorker_GravshipLaunch.TryExecuteOn))]
    public static class Patch_RitualBehaviorWorker_GravshipLaunch_TryExecuteOn
    {
        // 满足条件 → 执行“我的版本”（先照抄原版，再调用 base.TryExecuteOn）
        // 否则 → return true 执行原版
        public static bool Prefix(
            RitualBehaviorWorker_GravshipLaunch __instance,
            TargetInfo target,
            Pawn organizer,
            Precept_Ritual ritual,
            RitualObligation obligation,
            RitualRoleAssignments assignments,
            bool playerForced)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            if (MC == null)
            {
                return true;
            }


            try
            {
                // === 下面是“我的版本”，当前按原版逐字实现，你后续可微调 ===

                var engine = target.Thing?.TryGetComp<CompPilotConsole>()?.engine;
                if (engine == null) return false;

                // 访问本类的私有字段
                var fTmpPawns = AccessTools.Field(typeof(RitualBehaviorWorker_GravshipLaunch), "tmpPawns");
                var fForceVisitorsToLeave = AccessTools.Field(typeof(RitualBehaviorWorker_GravshipLaunch), "forceVisitorsToLeave");
                var fBoardColonyAnimals = AccessTools.Field(typeof(RitualBehaviorWorker_GravshipLaunch), "boardColonyAnimals");
                var fBoardColonyMechs = AccessTools.Field(typeof(RitualBehaviorWorker_GravshipLaunch), "boardColonyMechs");

                var tmpPawns = (List<Pawn>)(fTmpPawns.GetValue(__instance) ?? new List<Pawn>());
                bool forceVisitorsToLeave = (bool)(fForceVisitorsToLeave.GetValue(__instance) ?? false);
                bool boardColonyAnimals = (bool)(fBoardColonyAnimals.GetValue(__instance) ?? false);
                bool boardColonyMechs = (bool)(fBoardColonyMechs.GetValue(__instance) ?? false);

                engine.pawnsToBoard = new HashSet<Pawn>();
                engine.pawnsToLeave = new HashSet<Pawn>();

                tmpPawns.Clear();
                tmpPawns.AddRange(target.Map.mapPawns.AllPawnsSpawned);

                foreach (Pawn p in tmpPawns)
                {
                    if (p.Downed) continue;

                    if (forceVisitorsToLeave && p.Faction != null && p.Faction != Faction.OfPlayer && !p.Faction.HostileTo(Faction.OfPlayer))
                    {
                        engine.pawnsToLeave.Add(p);
                        p.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                    else if (boardColonyAnimals && p.IsColonyAnimal)
                    {
                        engine.pawnsToBoard.Add(p);
                        p.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                    else if (ModsConfig.BiotechActive && boardColonyMechs && p.IsColonyMech && !assignments.Participants.Any((Pawn Mech) => Mech == p))
                    {
                        engine.pawnsToBoard.Add(p);
                        p.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                }

                // 调用“基类”的 TryExecuteOn（不是当前覆写），避免递归原方法
                CallBase_TryExecuteOn(__instance, target, organizer, ritual, obligation, assignments, playerForced);

                return false; // 跳过原版
            }
            catch (Exception ex)
            {
                Log.Error($"[AAO][GravshipLaunch] custom TryExecuteOn failed: {ex}");
                return true; // 失败时放行原版兜底
            }
        }


        // === 关键：ReversePatch 到“基类 RitualBehaviorWorker.TryExecuteOn”的原始实现 ===
        [HarmonyReversePatch(HarmonyReversePatchType.Original)]
        [HarmonyPatch(typeof(RitualBehaviorWorker), nameof(RitualBehaviorWorker.TryExecuteOn))]
        static void CallBase_TryExecuteOn(object _ignored, TargetInfo target, Pawn organizer, Precept_Ritual ritual,
                                          RitualObligation obligation, RitualRoleAssignments assignments, bool playerForced)
        {
            // 这个方法体会在运行时被 Harmony 替换成对“基类原始实现”的直接调用
            throw new NotImplementedException("Stub for reverse patch");
        }
    }
}
