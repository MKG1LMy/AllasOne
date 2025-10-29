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
    public class AAO_Hediff_InfiniteResearch : HediffWithComps
    {
        public HediffStage curStage;
        public override HediffStage CurStage
        {
            get { return curStage; }
        }

    }
}
