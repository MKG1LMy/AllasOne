using AllasOne.WorldandGame;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.HediffCompMisc
{
    public class AAO_Hediff_BandwidthReinvention : Hediff
    {
        private int cachedTunedBandNodesCount
        {
            get
            {
                var BR = AAO_WorldComponent_BandwidthReinvention.Instance;
                if (BR.initBaseCost < 0)
                {
                    return 0;
                }
                return BR.researchCount * BR.bandwidthUp;
            }
        }
        private HediffStage curStage;

        public int AdditionalBandwidth => cachedTunedBandNodesCount;

        public override bool ShouldRemove => cachedTunedBandNodesCount == 0;

        public override HediffStage CurStage
        {
            get
            {
                var BR = AAO_WorldComponent_BandwidthReinvention.Instance;
                

                if (BR.ToUpdateBnadwidth && cachedTunedBandNodesCount > 0)
                {
                    StatModifier statModifier = new StatModifier();
                    statModifier.stat = StatDefOf.MechBandwidth;
                    statModifier.value = cachedTunedBandNodesCount;
                    curStage = new HediffStage();
                    curStage.statOffsets = new List<StatModifier> { statModifier };
                    BR.ToUpdateBnadwidth = false;
                   
                }
                return curStage;
            }
        }



    }
}
