using AllasOne.WorldandGame;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using static RimWorld.MechClusterSketch;
using AllasOne.WorkandJob;

namespace AllasOne.FloatMenuOptions
{
    public class FloatMenuOptionProvider_AAOCaptureMech : FloatMenuOptionProvider
    {
        // 放宽筛选：征召/未征召都行；机械族执行者也可（关闭 Manipulation 要求）
        protected override bool Drafted => true;  
        protected override bool Undrafted => true;
        protected override bool Multiselect => false;
        protected override bool RequiresManipulation => false;
        protected override bool MechanoidCanDo => true;
        protected override bool CanSelfTarget => true;

        protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext ctx)
        {
            var overseer = AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (overseer == null) return null;           // 没有你的机械意识，不显示
            if(ctx.FirstSelectedPawn.GetOverseer() !=overseer) return null;  //该机械族不是机械意识麾下
            if (clickedPawn == null || clickedPawn.Dead) return null;  // 无效目标
            if (!clickedPawn.RaceProps.IsMechanoid) return null;  //选中目标不是机械族

            if (clickedPawn.Faction == Faction.OfPlayer && !clickedPawn.IsColonyMechRequiringMechanitor()) return null;  //友方不需要控制的机械族包括本身不需要以及该控制已控制不考虑

            //以下情况只剩未受控友军及敌人

            if (!clickedPawn.Downed && !clickedPawn.IsColonyMechRequiringMechanitor()) // 未倒地且不是未受控友军-》未倒地敌人-》拒绝接管
                return new FloatMenuOption("AAO_CannotControlMech_LinkDenied".Translate(), null);

            //敌人已倒地，或者友军需要控制的机械族
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("AAO_TakeOverMech".Translate(clickedPawn), () =>
            {
                // 创建并派发接管任务
                Job job = JobMaker.MakeJob(AAO_JobDefOf.AAO_TakeOverMechSelf, clickedPawn, ctx.FirstSelectedPawn);
                job.count = 600; // 等待时长（ticks）
                ctx.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }),
            ctx.FirstSelectedPawn,
            clickedPawn
            );

        }
    }
}
