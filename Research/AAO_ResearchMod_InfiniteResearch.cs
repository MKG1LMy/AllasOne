using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllasOne.WorldandGame;
using RimWorld;
using Verse;
using   AllasOne.HediffCompMisc;

namespace AllasOne.Research
{
    public class AAO_ResearchMod_InfiniteResearch: ResearchMod
    {
        public ResearchProjectDef proj;
        public float initBaseCost;
        public float pointFactor;
        public HediffDef hediffDef;
        public bool ToMC = true;
        public bool ToMCmechanoids = false;


        public override void Apply()
        {
            var IRU = AAO_GameComp_InfiniteResearchUtility.Instance;
            if (IRU == null) return;
            var MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null && ToMC) return;


            //初始化或更新
            int idx = IRU.infiniteResearches.FindIndex(x => x.researchDef == proj);
            if (idx == -1)
            {
                var newRes = new AAO_GameComp_InfiniteResearchUtility.InfiniteResearch
                (
                    proj,
                    hediffDef,
                    0,
                    initBaseCost,
                    pointFactor
                    
                );
                IRU.infiniteResearches.Add(newRes);
            }
            else
            {
                var tmp = IRU.infiniteResearches[idx]; // 值类型，取出副本
                tmp.baseCost = initBaseCost;
                tmp.pointFactor = pointFactor;
                tmp.hediffDef = hediffDef; // 若需要也更新其它字段
                IRU.infiniteResearches[idx] = tmp; // 把修改后的副本写回列表
            }




            var manager = Find.ResearchManager;
            int idx2 = IRU.infiniteResearches.FindIndex(x => x.researchDef == proj);
            if (idx2 != -1)
            {
                var tmp = IRU.infiniteResearches[idx2]; // 取得副本（struct）

                // 若已完成研究，则重置进度，增加计数，更新成本
                if (proj.IsFinished)
                {
                    // 重置进度，增加计数
                    manager.AddProgress(proj, -manager.GetProgress(proj));

                    tmp.count++; // 修改副本
                    IRU.infiniteResearches[idx2] = tmp; // 写回列表

                    IRU.UpdateResearchCost(proj, tmp.baseCost, tmp.count, tmp.pointFactor);

                    // 给MC添加Hediff
                    if (MC != null && tmp.hediffDef != null && ToMC)
                    {
                        var hediff = MC.health.hediffSet.GetFirstHediffOfDef(tmp.hediffDef);
                        if (hediff == null)
                        {
                            hediff = HediffMaker.MakeHediff(tmp.hediffDef, MC);
                            MC.health.AddHediff(hediff);
                            var hediffComp = hediff.TryGetComp<AAO_HediffComp_InfiniteResearch>();
                            if (hediffComp != null) hediffComp.UpdateStage();
                        }
                        else
                        {
                            var hediffComp = hediff.TryGetComp<AAO_HediffComp_InfiniteResearch>();
                            if (hediffComp != null) hediffComp.UpdateStage();
                        }
                    }
                    // 给MC控制的机械族添加Hediff
                    else if (MC != null && tmp.hediffDef != null && ToMCmechanoids)
                    {
                        List<Pawn> mechanoids = MC.mechanitor?.ControlledPawns; 
                        if (mechanoids != null)
                        {
                            foreach (var mech in mechanoids)
                            {
                                var hediff = mech.health.hediffSet.GetFirstHediffOfDef(tmp.hediffDef);
                                if (hediff == null)
                                {
                                    hediff = HediffMaker.MakeHediff(tmp.hediffDef, mech);
                                    mech.health.AddHediff(hediff);
                                    var hediffComp = hediff.TryGetComp<AAO_HediffComp_InfiniteResearch>();
                                    if (hediffComp != null) hediffComp.UpdateStage();
                                }
                                else
                                {
                                    var hediffComp = hediff.TryGetComp<AAO_HediffComp_InfiniteResearch>();
                                    if (hediffComp != null) hediffComp.UpdateStage();
                                }
                            }
                        }
                    }


                }


                // 每次都会更新Hediff的阶段（加载存档阶段也要更新状态）
                if (MC != null && tmp.hediffDef != null && ToMC)
                {
                    var hediff = MC.health.hediffSet.GetFirstHediffOfDef(tmp.hediffDef);
                    if (hediff != null)
                    {
                        var hediffComp = hediff.TryGetComp<AAO_HediffComp_InfiniteResearch>();
                        if (hediffComp != null) hediffComp.UpdateStage();
                    }
                }
                else if (MC != null && tmp.hediffDef != null && ToMCmechanoids)
                {
                    List<Pawn> mechanoids = MC.mechanitor?.ControlledPawns;
                    if (mechanoids != null)
                    {
                        foreach (var mech in mechanoids)
                        {
                            var hediff = mech.health.hediffSet.GetFirstHediffOfDef(tmp.hediffDef);
                            if (hediff != null)
                            {
                                var hediffComp = hediff.TryGetComp<AAO_HediffComp_InfiniteResearch>();
                                if (hediffComp != null) hediffComp.UpdateStage();
                            }
                        }
                    }
                }



            }

        }



    }
}
