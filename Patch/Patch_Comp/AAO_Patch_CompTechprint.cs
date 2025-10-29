using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AllasOne.Patch.Patch_Comp
{
    [HarmonyPatch(typeof(CompTechprint), "CompFloatMenuOptions")]
    public static class Patch_CompTechprint_CompFloatMenuOptions_Prefix
    {
        // Prefix 拦截返回 IEnumerable<FloatMenuOption>
        public static bool Prefix(CompTechprint __instance, Pawn selPawn, ref IEnumerable<FloatMenuOption> __result)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            try
            {
                if (selPawn == null) return true; // 不干预
                if (!selPawn.RaceProps.IsMechanoid) return true;
                if(selPawn?.GetOverseer() != MC) return true;

                // 构造自定义菜单项集合并替换返回值
                __result = BuildMechOptions(__instance, selPawn);
                return false; // 跳过原始方法
            }
            catch (Exception ex)
            {
                Log.Error($"[Patch_CompTechprint] Prefix error: {ex}");
                return true; // 失败时放行原方法以保证稳定
            }
        }

        // ===== 在这里实现你的机械族专用菜单生成逻辑 =====
        private static IEnumerable<FloatMenuOption> BuildMechOptions(CompTechprint comp, Pawn selPawn)
        {
            var parent = comp.parent; // the techprint thing

            if (!ModLister.CheckRoyalty("Techprint"))
            {
                yield break;
            }
            JobFailReason.Clear();
            if (!selPawn.CanReach(parent, PathEndMode.ClosestTouch, Danger.Some))
            {
                JobFailReason.Is("CannotReach".Translate());
            }

            HaulAIUtility.PawnCanAutomaticallyHaul(selPawn, parent, forced: true);
            Thing bench = GenClosest.ClosestThingReachable(
                selPawn.Position, selPawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.ResearchBench),
                PathEndMode.InteractionCell,
                TraverseParms.For(selPawn, Danger.Some),
                9999f,
                thing2 => thing2 is Building_ResearchBench && !thing2.IsForbidden(selPawn) && selPawn.CanReserve(thing2)
            );

            Job job = null;
            if (bench != null)
            {
                job = JobMaker.MakeJob(JobDefOf.ApplyTechprint);
                job.targetA = bench;
                job.targetB = parent;
                job.targetC = bench.Position;
            }
            if (JobFailReason.HaveReason)
            {
                yield return new FloatMenuOption(
                    "CannotGenericWorkCustom".Translate("ApplyTechprint".Translate(parent.Label)) + ": " + JobFailReason.Reason.CapitalizeFirst(),
                    null
                );
                JobFailReason.Clear();
                yield break;
            }

            // 主动任务项：点击后下达任务或给出提示（和原版行为保持一致）
            yield return FloatMenuUtility.DecoratePrioritizedTask(
                new FloatMenuOption(
                    "ApplyTechprint".Translate(parent.Label).CapitalizeFirst(),
                    delegate
                    {
                        if (job == null)
                        {
                            Messages.Message("MessageNoResearchBenchForTechprint".Translate(), MessageTypeDefOf.RejectInput);
                        }
                        else
                        {
                            selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        }
                    }
                ),
                selPawn, parent
            );


        }
    }
}
