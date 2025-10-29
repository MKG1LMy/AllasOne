using AllasOne.WorldandGame;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Research
{
    public class AAO_BandwidthReinvention_ResearchMod : ResearchMod
    {
        public ResearchProjectDef proj = AAO_ResearchProjectDefOf.AAO_BandwidthReinvention;
        public float initBaseCost;
        public int bandwidthUp;


        public override void Apply()
        {
            var BR = AAO_WorldComponent_BandwidthReinvention.Instance;
            if (BR == null) return;

            var GBR = AAO_GameComponent_BandwidthReinvention.Instance;
            if (GBR == null) return;

            BR.initBaseCost = initBaseCost;
            BR.proj = proj;
            BR.bandwidthUp = bandwidthUp;
            GBR.initBaseCost = initBaseCost;
            GBR.proj = proj;


            ResearchManager manager = Find.ResearchManager;
            // ----------- 无限研究逻辑 -----------
            if (proj.IsFinished)
            {
                // 重置进度，增加计数
                manager.AddProgress(proj, -(manager.GetProgress(proj)));
                BR.researchCount++;
                BR.UpdateResearchCost();

                var MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
                if (MC != null && !MC.Dead)
                {
                    var hediff = MC.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("AAO_Hediff_BandwidthReinvention"));
                    if (hediff == null)
                    {
                        hediff = HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("AAO_Hediff_BandwidthReinvention"), MC);
                        MC.health.AddHediff(hediff);
                    }

                    BR.ToUpdateBnadwidth = true;

                }
            }

        }
    }


}
