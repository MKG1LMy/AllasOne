using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AllasOne.BuildingComp
{
    public class AAO_Building_PlaceWorker_ShowRemoteCharge : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            AAO_Building_CompProperties_RemoteCharge compProperties = def.GetCompProperties<AAO_Building_CompProperties_RemoteCharge>();
            if (compProperties != null)
            {
                GenDraw.DrawRadiusRing(center, compProperties.range, Color.white);
            }
        }
    }
}
