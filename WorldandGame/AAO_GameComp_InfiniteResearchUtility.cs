using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.WorldandGame
{
    public class AAO_GameComp_InfiniteResearchUtility : GameComponent
    {
        public class InfiniteResearch : IExposable
        {
            public ResearchProjectDef researchDef;
            public HediffDef hediffDef;
            public int count;
            public float baseCost;
            public float pointFactor;

            public InfiniteResearch() { } // 必须有

            public InfiniteResearch(ResearchProjectDef researchDef, HediffDef hediffDef, int count, float baseCost, float pointFactor)
            {
                this.researchDef = researchDef;
                this.hediffDef = hediffDef;
                this.count = count;
                this.baseCost = baseCost;
                this.pointFactor = pointFactor;
            }

            public void ExposeData()
            {
                // Def 类型用 Scribe_Defs，引用类型（Thing/ThingWithComps）用 Scribe_References
                Scribe_Defs.Look(ref researchDef, "researchDef");
                Scribe_Defs.Look(ref hediffDef, "hediffDef");
                Scribe_Values.Look(ref count, "count", 0);
                Scribe_Values.Look(ref baseCost, "baseCost", 0f);
                Scribe_Values.Look(ref pointFactor, "pointFactor", 1f);
            }
        }




        public AAO_GameComp_InfiniteResearchUtility(Game game) : base() { }
        public static AAO_GameComp_InfiniteResearchUtility Instance => Current.Game?.GetComponent<AAO_GameComp_InfiniteResearchUtility>();

        public List<InfiniteResearch> infiniteResearches = new List<InfiniteResearch>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref infiniteResearches, "infiniteResearches",LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ApplyAllMods();
                UpdateAllResearchCosts();
            }
        }

        //新建游戏时初始化所有研究
        public override void StartedNewGame()
        {
            base.StartedNewGame();
            ApplyAllMods();
            RenewAllResearchCosts();
        }

        //更新当前研究成本
        public void UpdateResearchCost(ResearchProjectDef proj , float baseCost, int count, float pointFactor)
        {
            if (proj == null) return;
            if(baseCost <= 0) return;
            float newCost = baseCost * (float)Math.Pow(1.5, count) * pointFactor; //每次研究成本翻倍 
            proj.baseCost = (int)newCost;
        }

        //更新所有研究成本
        public void UpdateAllResearchCosts()
        {
            if (infiniteResearches == null) return;
            var copy = infiniteResearches.ToList(); // 副本
            foreach (var res in copy)
            {
                var proj = res.researchDef;
                if (proj == null) continue;
                proj.ReapplyAllMods();
                UpdateResearchCost(proj, res.baseCost, res.count, res.pointFactor);
            }
        }

        //重置所有研究成本
        public void RenewAllResearchCosts()
        {
            if (infiniteResearches == null) return;
            var copy = infiniteResearches.ToList(); // 副本
            foreach (var res in copy)
            {
                var proj = res.researchDef;
                if (proj == null) continue;
                proj.ReapplyAllMods();
                proj.baseCost = res.baseCost;
                Log.Message("AAO Infinite Research: Initialized research " + proj.defName + " with base cost " + res.baseCost);
            }
        }


        //应用所有研究的修改
        public void ApplyAllMods()
        {
            foreach (ResearchProjectDef allDef in DefDatabase<ResearchProjectDef>.AllDefs)
            {
               allDef.ReapplyAllMods();
            }
        }



    }








    


}
