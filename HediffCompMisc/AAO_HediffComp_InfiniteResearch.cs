using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;
using AllasOne.WorldandGame;

namespace AllasOne.HediffCompMisc
{
    public class AAO_HediffComp_InfiniteResearch : HediffComp
    {
        public AAO_HediffCompProperties_InfiniteResearch Props => (AAO_HediffCompProperties_InfiniteResearch)props;

        public void UpdateStage()
        {
            var IRU = AllasOne.WorldandGame.AAO_GameComp_InfiniteResearchUtility.Instance;
            if (IRU == null) return;
            AAO_GameComp_InfiniteResearchUtility.InfiniteResearch res = IRU.infiniteResearches.FirstOrDefault(x => x.hediffDef == parent.def);
            if (res.hediffDef == null) return;           
            AAO_Hediff_InfiniteResearch HediffIR = (AAO_Hediff_InfiniteResearch)parent;
            if (HediffIR == null) return;

            HediffStage newStages = new HediffStage();

            //计算修正
            if (Props.statOffsets != null)
            {
                List<StatModifier> newStatOffsets = new List<StatModifier>();
                foreach (var statOffset in Props.statOffsets)
                {
                    StatModifier newStat = new StatModifier();
                    newStat.stat = statOffset.stat;
                    newStat.value = statOffset.value * (res.count) * Props.statfactor; // 根据计数调整数值
                    newStatOffsets.Add(newStat);
                }
                newStages.statOffsets = newStatOffsets;
            }
            if (Props.statFactors != null)
            {
                List<StatModifier> newStatFactors = new List<StatModifier>();
                foreach (var statFactor in Props.statFactors)
                {
                    StatModifier newStat = new StatModifier();
                    newStat.stat = statFactor.stat;
                    newStat.value = (float)Math.Pow(statFactor.value, res.count) * Props.statfactor; // 根据计数调整数值
                    newStatFactors.Add(newStat);
                }
                newStages.statFactors = newStatFactors;
            }

            HediffIR.curStage = newStages;

        }

    }
}

