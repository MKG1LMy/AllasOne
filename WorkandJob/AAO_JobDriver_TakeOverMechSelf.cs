using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;


namespace AllasOne.WorkandJob
{
    public class AAO_JobDriver_TakeOverMechSelf : JobDriver
    {
        private Pawn Mech => job.targetA.Pawn;
        private Pawn Mymech => job.targetB.Pawn;
        private int WaitTicks => job.count > 0 ? job.count : 180;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            // 目标自己执行，不需要预定他物；返回true即可
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn Overseer = Mymech.GetOverseer();

            // 等待阶段：mech 原地等待 WaitTicks，并在 mech 头上显示进度条
            var wait = new Toil
            {
                initAction = () =>
                {
                    // 原地站定，别走动
                    var actor = Mech;
                    actor.pather?.StopDead();
                    PawnUtility.ForceWait(Mech, WaitTicks, Mymech);
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = WaitTicks
            };

            
            wait.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);

            yield return wait;


            var convert = new Toil
            {
                initAction = () =>
                {
                    // 关系组件保证存在
                    if (Mech.relations == null)
                        Mech.relations = new Pawn_RelationsTracker(Mech);

                    // 先转阵营
                    if (Mech.Faction != Faction.OfPlayer)
                        Mech.SetFaction(Faction.OfPlayer);

                    // 再绑定 Overseer（你已有的 AddDirectRelation Postfix 会处理技能重定向）
                    if (!Mech.relations.DirectRelationExists(PawnRelationDefOf.Overseer, Overseer))
                        Mech.relations.AddDirectRelation(PawnRelationDefOf.Overseer, Overseer);

                    // 完成
                    ReadyForNextToil();
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return convert;
        }
    }
}
