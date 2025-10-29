using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.BuildingComp
{
    public class AAO_Building_ComProperties_ComputingCenter : CompProperties
    {
        public bool ResearchByMc = false;

        public float ResearchSpeedFactor = 1.0f;

        public int checkInterval = 600;

        public float researchPoint = 1;

        public AAO_Building_ComProperties_ComputingCenter()
        {
            compClass = typeof(AAO_Building_ComputingCenter);
        }
    }
}
