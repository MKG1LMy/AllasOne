using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.BuildingComp
{
    public class AAO_Building_CompProperties_RemoteCharge: CompProperties
    {

        public float range;

        public int checkInterval = 10;

        public int ticksTocharge = 180;

        public SoundDef activeSound;

        public bool drawLines = true;

        public float energyCharge = 1.5f;

        public int maxMech = 10;

        public AAO_Building_CompProperties_RemoteCharge()
        {
            compClass = typeof(AAO_Building_CompRemoteCharge);
        }
    }
}
