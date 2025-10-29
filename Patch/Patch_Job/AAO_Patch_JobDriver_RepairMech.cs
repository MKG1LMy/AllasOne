using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllasOne.Patch.Patch_Job
{
    using HarmonyLib;
    using RimWorld;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Verse;
    using Verse.AI;

    [HarmonyPatch(typeof(JobDriver_RepairMech), "MakeNewToils")]
    public static class JobDriver_RepairMech_MakeNewToils_Patch
    {
        // Prefix 替换原方法的返回值（IEnumerable<Toil>）
        public static bool Prefix(JobDriver_RepairMech __instance, ref IEnumerable<Toil> __result)
        {
            try
            {
                // 获取 actor（执行者）
                Pawn actor = __instance.pawn;

                // 通过反射获取 Mech 属性或字段（兼容 private/protected）
                PropertyInfo propMech = AccessTools.Property(typeof(JobDriver_RepairMech), "Mech");
                FieldInfo fieldMech = AccessTools.Field(typeof(JobDriver_RepairMech), "Mech");
                Pawn mech = null;
                if (propMech != null) mech = (Pawn)propMech.GetValue(__instance);
                else if (fieldMech != null) mech = (Pawn)fieldMech.GetValue(__instance);

                // 如果没能获取到 mech，则不干预，执行原方法
                if (mech == null) return true;

                // 若执行者与目标相同，则用自定义实现替代原方法
                if (ReferenceEquals(actor, mech))
                {
                    __result = CustomSelfRepairToils(__instance, actor, mech);
                    Log.Message("[Patch JobDriver_RepairMech] Applied custom self-repair toils.");
                    return false; // 跳过原方法
                }

                // 否则不干预，执行原方法
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[Patch JobDriver_RepairMech] Prefix failed: {ex}");
                // 出错时保守起见让原方法继续执行
                return true;
            }
        }

        // 自定义 Toil 序列（针对 self-repair 的安全实现）
        private static IEnumerable<Toil> CustomSelfRepairToils(JobDriver_RepairMech driver, Pawn pawn, Pawn Mech)
        {
            // 读取一些可能为私有的字段：Remote, TicksPerHeal
            FieldInfo fRemote = AccessTools.Field(typeof(JobDriver_RepairMech), "Remote");
            FieldInfo fTicksPerHeal = AccessTools.Field(typeof(JobDriver_RepairMech), "TicksPerHeal");

            bool Remote = fRemote != null && (bool)fRemote.GetValue(driver);
            int TicksPerHeal = fTicksPerHeal != null ? (int)fTicksPerHeal.GetValue(driver) : 60; // 回退默认值

            // 与原方法逻辑一致的前置检查（如果需要可在此扩展）
            if (!ModLister.CheckBiotech("Mech repair"))
            {
                yield break;
            }

            // FailOn* 的行为无法在这里完全复制为编译时约束，但大多数 FailOn 仅在 job 执行时起作用。
            // 我们保留原来行为的主要修正点：构造 Wait Toil 并在 FinishAction 中避免结束自身 job。

            if (!Remote)
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            }

            // 本地化变量，供闭包使用
            int ticksToNextRepair = TicksPerHeal;
            int TicksPerHealLocal = TicksPerHeal;

            Toil toil = (Toils_General.Wait(int.MaxValue));
            toil.WithEffect(EffecterDefOf.MechRepairing, TargetIndex.A);
            toil.PlaySustainerOrSound(Remote ? SoundDefOf.RepairMech_Remote : SoundDefOf.RepairMech_Touch);
            toil.AddPreInitAction(delegate
            {
                ticksToNextRepair = TicksPerHealLocal;
            });
            toil.handlingFacing = true;

            // tickIntervalAction：采取与原版相同的修理与技能增长逻辑
            toil.tickIntervalAction = delegate (int delta)
            {
                try
                {
                    ticksToNextRepair -= delta;
                    if (ticksToNextRepair <= 0)
                    {
                        // 修改目标能量并执行修理
                        if (Mech.needs?.energy != null)
                        {
                            Mech.needs.energy.CurLevel -= Mech.GetStatValue(StatDefOf.MechEnergyLossPerHP) * (float)delta;
                        }
                        MechRepairUtility.RepairTick(Mech, delta);
                        ticksToNextRepair = TicksPerHealLocal;
                    }
                    pawn.rotationTracker.FaceTarget(Mech);
                    if (pawn.skills != null)
                    {
                        pawn.skills.Learn(SkillDefOf.Crafting, 0.05f * (float)delta);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"[Patch JobDriver_RepairMech] tickIntervalAction exception: {ex}");
                }
            };

            // 修正后的 FinishAction：若目标和执行者是同一人则不要结束其当前 job
            toil.AddFinishAction(delegate
            {
                try
                {
                    if (Mech == null) return;

                    // 仅在目标不是修理者本身的情况下结束目标的当前 job
                    if (!ReferenceEquals(Mech, pawn))
                    {
                        if (Mech.jobs?.curJob != null)
                        {
                            Mech.jobs.EndCurrentJob(JobCondition.InterruptForced);
                        }
                    }
                    else
                    {
                        
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"[Patch JobDriver_RepairMech] FinishAction exception: {ex}");
                }
            });

            toil.AddEndCondition(() => MechRepairUtility.CanRepair(Mech) ? JobCondition.Ongoing : JobCondition.Succeeded);

            if (!Remote)
            {
                toil.activeSkill = () => SkillDefOf.Crafting;
            }

            yield return toil;
        }
    }
}
