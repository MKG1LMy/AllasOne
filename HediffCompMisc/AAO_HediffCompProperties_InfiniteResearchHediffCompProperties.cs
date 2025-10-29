using AllasOne.WorldandGame;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Verse;

namespace AllasOne.HediffCompMisc
{
    public class AAO_HediffCompProperties_InfiniteResearch : HediffCompProperties
    {
        public AAO_HediffCompProperties_InfiniteResearch()
        {
            compClass = typeof(AAO_HediffComp_InfiniteResearch);
        }

        public List<StatModifier> statOffsets;

        public List<StatModifier> statFactors;

        public float statfactor = 1;
    }
}
