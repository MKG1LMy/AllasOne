using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld.Planet;


namespace AllasOne.WorldandGame
{
    public class AAO_WorldComponent_ShowMechOnBar : WorldComponent
    {
        public AAO_WorldComponent_ShowMechOnBar(World world) : base(world) { }

        public static AAO_WorldComponent_ShowMechOnBar Instance => Find.World?.GetComponent<AAO_WorldComponent_ShowMechOnBar>();

        public List<int> thingNumber = new List<int>(); 

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref thingNumber, "AAO_ShowMechOnBar_thingNumber", LookMode.Value);
        }

    }
}
