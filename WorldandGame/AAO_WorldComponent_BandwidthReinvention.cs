using AllasOne.Research;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.WorldandGame
{

    //世界方法管理当前的研究次数和研究成本
    public class AAO_WorldComponent_BandwidthReinvention : WorldComponent
    {
        public AAO_WorldComponent_BandwidthReinvention(World world) : base(world) { }

        public static AAO_WorldComponent_BandwidthReinvention Instance => Find.World?.GetComponent<AAO_WorldComponent_BandwidthReinvention>();

        public int researchCount = 0; //当前研究次数
        public ResearchProjectDef proj = AAO_ResearchProjectDefOf.AAO_BandwidthReinvention; //对应的研究项目
        public float initBaseCost = -1; //初始基础研究点
        public int bandwidthUp = -1; //每次研究增加的带宽

        public bool ToUpdateBnadwidth =  false ;

        public void UpdateResearchCost()
        {
            if (proj == null) return;
            if (initBaseCost < 0) return;
            float newCost = initBaseCost * (float)Math.Pow(1.5, researchCount); //每次研究成本翻倍 
            //float newCost = initBaseCost + 500 * researchCount; 
            proj.baseCost = (int)newCost;

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref researchCount, "AAO_BandwidthReinvention_researchCount");
            Scribe_Values.Look(ref initBaseCost, "AAO_BandwidthReinvention_initBaseCost");
            Scribe_Values.Look(ref bandwidthUp, "AAO_BandwidthReinvention_bandwidthUp");
            Scribe_Defs.Look(ref proj, "AAO_BandwidthReinvention_proj");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                proj.ReapplyAllMods();
                UpdateResearchCost();
                ToUpdateBnadwidth = true;
            }
        }

    }





    //游戏全局方法防止新建游戏数据被旧存档覆盖
    public class AAO_GameComponent_BandwidthReinvention : GameComponent
    {
        public AAO_GameComponent_BandwidthReinvention(Game game) : base() { }
        public static AAO_GameComponent_BandwidthReinvention Instance => Current.Game?.GetComponent<AAO_GameComponent_BandwidthReinvention>();

        public ResearchProjectDef proj = AAO_ResearchProjectDefOf.AAO_BandwidthReinvention;//对应的研究项目
        public float initBaseCost; //初始基础研究点

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            proj.ReapplyAllMods();
            proj.baseCost = (int)initBaseCost;
        }

    }



}
