using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

namespace AllasOne.HediffCompMisc
{
    public class AAO_Hediff_MechanoidConsciousnessNode : HediffWithComps
    {
        //自动研究相关
        private int tickAcc = 0; // 用于计时
        public int TicksToReserach = 600; // 每多少时间研究一次
        public float BasePerBandwidthPerSec = 0.1f;// 基础系数：每秒、每点已用带宽换多少研究点；按需调大/调小
        public float LearningSpeed = 5.0f; // 学习速度系数(每带宽使得每次研究增加的学习进度)，按需调大/调小
        public int checkInterval = 10; //Hash时间
        public int TickCountToResearch = 60;//计时次数，为计时研究时长除hash间隔
        public float SaveResearchPoint = 0;//贮存溢出研究点
        private ResearchProjectDef Project => Find.ResearchManager.GetProject();

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            if (pawn.Spawned)
            {
                var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
                mgr.MechanoidConsciousness = pawn;

                if (pawn.Faction != Faction.OfPlayer) pawn.SetFaction(Faction.OfPlayer);

                if (pawn.Spawned) pawn.DeSpawn(); // 只退场，不销毁

                //// 清理可能的借宿/囚犯状态（避免读档后被系统处理成其他阵营）
                //Find.WorldPawns.PassToWorld(pawn);

                //pawn.SetFaction(Faction.OfPlayer); // 再保险一次

                LinkAllControlledMechsToOverseer(pawn);
                Find.ColonistBar.MarkColonistsDirty();
            }
        }

        //开局初始化
        public override void Notify_Spawned()
        {
            base.Notify_Spawned();
            var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            mgr.MechanoidConsciousness = pawn;

            if (pawn.Faction != Faction.OfPlayer) pawn.SetFaction(Faction.OfPlayer);

            if (pawn.Spawned) pawn.DeSpawn(); // 只退场，不销毁

            //// 清理可能的借宿/囚犯状态（避免读档后被系统处理成其他阵营）
            //Find.WorldPawns.PassToWorld(pawn);

            //pawn.SetFaction(Faction.OfPlayer); // 再保险一次

            LinkAllControlledMechsToOverseer(pawn);

        }

        //绑定所有机械族技能到Overseer
        private static void LinkAllControlledMechsToOverseer(Pawn overseer)
        {
            if (overseer == null) return;

            List<Pawn> controlled = overseer?.mechanitor?.ControlledPawns;
            if (controlled.NullOrEmpty()) return;

            foreach (var mech in controlled)
            {

                LinkMechToOverseer(overseer, mech);
                
            }
        }

        //绑定机械族技能到Overseer
        public static void LinkMechToOverseer(Pawn overseer, Pawn mech)
        {
            if (overseer == null || mech == null) return;

            if (mech.skills == null)
            {
                mech.skills = new Pawn_SkillTracker(mech);
            }
            mech.skills.skills = overseer.skills.skills; //连接overseer的技能

            //Log.Message($"[AllasOne] Linked mech skills: {mech.LabelShort} (#{mech.thingIDNumber}) " +$"→ {overseer.LabelShort} (#{overseer.thingIDNumber})");
            if (mech.interactions == null)
            {
                mech.interactions = new Pawn_InteractionsTracker(mech);
            }

            if (mech.royalty == null)
            {
                mech.royalty = new Pawn_RoyaltyTracker(mech);
                mech.royalty = overseer.royalty;
            }

            if (mech.story == null) //连接overseer的traits
            {
                mech.story = new Pawn_StoryTracker(mech);
                mech.story.traits = overseer.story.traits;
                mech.story.Childhood = overseer.story.Childhood;
                mech.story.Adulthood = overseer.story.Adulthood;
            }

            //Log.Message($"[AllasOne] Linked mech traits: {mech.LabelShort} (#{mech.thingIDNumber}) " +$"→ {overseer.LabelShort} (#{overseer.thingIDNumber})");

            if (mech.genes == null)
            {
                // 初始化机械族的 GeneTracker
                mech.genes = new Pawn_GeneTracker(mech);
                mech.genes.xenotypeName = "AAO_Mechanoid".Translate();

                // 若监管者有基因则将其基因复制给机械族（仅初始化一次）
                if (overseer.genes != null && overseer.genes.Xenogenes != null)
                {
                    foreach (var gene in overseer.genes.Xenogenes)
                    {
                        // 仅复制基因定义，不复制引用，避免共享同一实例
                        Gene newGene = GeneMaker.MakeGene(gene.def, mech);
                        mech.genes.AddGene(gene.def, true);
                    }
                    //Log.Message($"[AllasOne] Initialized mech genes from overseer: {mech.LabelShort} ← {overseer.LabelShort}");
                }

            }
        }

        //新加机械族初始化
        public override void Notify_RelationAdded(Pawn otherPawn, PawnRelationDef relationDef)
        {
            // 只处理 Overseer 关系
            if (relationDef != PawnRelationDefOf.Overseer || otherPawn == null) return;

            LinkMechToOverseer(pawn,otherPawn);            
        }

        //每秒
        public override void PostTick()
        {
            base.PostTick();
            if (pawn.IsHashIntervalTick(checkInterval))
            {
                DoResearchByMechConscious();
            }


        }

        //研究具体步骤
        private void DoResearchByMechConscious()
        {
            tickAcc++;
            if (tickAcc == TickCountToResearch)
            {
                tickAcc = 0;
                int CanUseBand = (pawn.mechanitor?.TotalBandwidth - pawn.mechanitor?.UsedBandwidth) ?? 0;                
                float statValue = pawn.GetStatValue(StatDefOf.ResearchSpeed);
                float researchGained = BasePerBandwidthPerSec * CanUseBand * 121.0f;

                if (researchGained > 0 && Project != null)
                {                    
                    Find.ResearchManager.ResearchPerformed(statValue * researchGained, pawn);
                    pawn.skills.Learn(SkillDefOf.Intellectual, LearningSpeed * CanUseBand);
                    //Log.Message($"[AllasOne] MechConscious research gain: {(float)(BasePerBandwidthPerSec * statValue * UsedBand)} (UsedBand: {UsedBand}) TargetProject:{Project.label}");
                }
                else if (researchGained > 0 && Project == null)
                {
                    SaveResearchPoint += statValue * researchGained;
                    pawn.skills.Learn(SkillDefOf.Intellectual, LearningSpeed * CanUseBand);
                    //Log.Message($"[AllasOne] MechConscious research save SaveResearchPoint: {(float)(BasePerBandwidthPerSec * statValue * UsedBand)} (UsedBand: {UsedBand}) NowHavePoing:{SaveResearchPoint / 121.0f}");
                }

                if (Project != null && SaveResearchPoint > 0)
                {
                    Find.ResearchManager.ResearchPerformed(SaveResearchPoint, pawn);
                    //Log.Message($"[AllasOne] MechConscious research gain use SaveResearchPoint: {(float)(SaveResearchPoint/121.0f)} (UsedBand: {UsedBand}) TargetProject:{Project.label}");
                    SaveResearchPoint = 0;
                }
            }

        }

        //设置研究参数
        public void SetResearchNum(int ticksToReserach, float basePerBandwidthPerSec, float learningSpeed)
        {
            TicksToReserach = ticksToReserach;
            BasePerBandwidthPerSec = basePerBandwidthPerSec;
            LearningSpeed = learningSpeed;
            TickCountToResearch = TicksToReserach / checkInterval;
        }

        //储存关键数据
        public override void ExposeData()
        {
            base.ExposeData();

            // 第三个参数用“当前字段值”，避免旧存档缺键时回到常量
            Scribe_Values.Look(ref TicksToReserach, "ticksToResearch", TicksToReserach, forceSave: true);
            Scribe_Values.Look(ref BasePerBandwidthPerSec, "basePerBandwidthPerSec", BasePerBandwidthPerSec, forceSave: true);
            Scribe_Values.Look(ref LearningSpeed, "learningSpeed", LearningSpeed, forceSave: true);
            Scribe_Values.Look(ref tickAcc, "tickAcc", tickAcc, forceSave: true);
            Scribe_Values.Look(ref TickCountToResearch, "TickCountToResearch", TickCountToResearch, forceSave: true);
            Scribe_Values.Look(ref SaveResearchPoint, "TickCountToResearch", SaveResearchPoint, forceSave: true);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                LinkAllControlledMechsToOverseer(pawn);               
            }

        }

    }
}
